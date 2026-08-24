using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SuperMarket.Application.Common;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.DTOs.Account;
using SuperMarket.Application.DTOs.Users;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Enums;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Identity;
using SuperMarket.Application.Common.Extensions;

namespace SuperMarket.Infrastructure.Services;

public sealed class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher _passwordHasher;


    public AccountService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IPasswordHasher passwordHasher)
    {
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(signInManager);
        ArgumentNullException.ThrowIfNull(userRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(emailService);
        ArgumentNullException.ThrowIfNull(passwordHasher);

        _userManager = userManager;
        _signInManager = signInManager;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _passwordHasher = passwordHasher;
    }



    // ============================================================
    // Register
    // ============================================================

    public async Task<Result<RegisterResultDto>> RegisterAsync(
        RegisterDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);


        if (dto.Password != dto.ConfirmPassword)
        {
            return Result<RegisterResultDto>
                .Failure("Passwords do not match.");
        }



        if (await _userRepository.ExistsByEmailAsync(
                dto.Email,
                cancellationToken: cancellationToken))
        {
            return Result<RegisterResultDto>
                .Failure("Email is already in use.");
        }



        var passwordHash =
            _passwordHasher.HashPassword(dto.Password);



        var domainUser =
            new User(
                dto.FirstName,
                dto.LastName,
                dto.Email,
                passwordHash);



        Result<RegisterResultDto>? result = null;



        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _userRepository.AddAsync(
                domainUser,
                cancellationToken);



            await _unitOfWork.SaveChangesAsync(
                cancellationToken);



            var identityUser =
                ApplicationUser.Create(
                    domainUser.Id,
                    dto.Email);



            var createResult =
                await _userManager.CreateAsync(
                    identityUser,
                    dto.Password);



            if (!createResult.Succeeded)
            {
                result =
                    Result<RegisterResultDto>.Failure(
                        createResult.Errors
                            .Select(e => e.Description));

                return;
            }



            var roleResult =
                await _userManager.AddToRoleAsync(
                    identityUser,
                    UserRole.Customer.ToRoleName());



            if (!roleResult.Succeeded)
            {
                result =
                    Result<RegisterResultDto>.Failure(
                        roleResult.Errors
                            .Select(e => e.Description));

                return;
            }



            result =
                Result<RegisterResultDto>.Success(
                    new RegisterResultDto
                    {
                        UserId = domainUser.Id,
                        Email = domainUser.Email.Value,
                        FullName = domainUser.Name.ToString(),
                        RequiresEmailConfirmation = false
                    });


        },
        cancellationToken);



        return result!;
    }




    // ============================================================
    // Login
    // ============================================================

    public async Task<Result<LoginResultDto>> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);



        var identityUser =
            await _userManager.FindByEmailAsync(
                dto.Email.Trim().ToLowerInvariant());



        if (identityUser is null)
        {
            return Result<LoginResultDto>
                .Failure("Invalid email or password.");
        }



        if (await _userManager.IsLockedOutAsync(identityUser))
        {
            return Result<LoginResultDto>.Success(
                new LoginResultDto
                {
                    IsLockedOut = true
                });
        }



        var signInResult =
            await _signInManager.PasswordSignInAsync(
                identityUser,
                dto.Password,
                dto.RememberMe,
                lockoutOnFailure: true);



        if (signInResult.IsLockedOut)
        {
            return Result<LoginResultDto>.Success(
                new LoginResultDto
                {
                    IsLockedOut = true
                });
        }



        if (signInResult.RequiresTwoFactor)
        {
            return Result<LoginResultDto>.Success(
                new LoginResultDto
                {
                    RequiresTwoFactor = true
                });
        }



        if (!signInResult.Succeeded)
        {
            return Result<LoginResultDto>
                .Failure("Invalid email or password.");
        }



        var domainUser =
            await _userRepository.GetByEmailAsync(
                dto.Email,
                cancellationToken);



        if (domainUser is null)
        {
            await _signInManager.SignOutAsync();

            return Result<LoginResultDto>
                .Failure(
                    "Your account is incomplete. Please contact support.");
        }

        // PasswordSignInAsync above already signs the user in with the
        // DomainUserId claim (see ApplicationUserClaimsPrincipalFactory).

        return Result<LoginResultDto>.Success(
            new LoginResultDto
            {
                UserId = domainUser.Id,
                Email = domainUser.Email.Value,
                FullName = domainUser.Name.ToString(),
                Role = domainUser.Role
            });
    }

    // ============================================================
    // Logout
    // ============================================================

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }



    // ============================================================
    // Change Password
    // ============================================================

    public async Task<Result> ChangePasswordAsync(
        ChangePasswordDto dto,
        Guid domainUserId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);



        if (domainUserId == Guid.Empty)
        {
            return Result.Failure(
                "User identity could not be determined.");
        }



        if (dto.NewPassword != dto.ConfirmPassword)
        {
            return Result.Failure(
                "Passwords do not match.");
        }



        if (dto.CurrentPassword == dto.NewPassword)
        {
            return Result.Failure(
                "New password must differ from the current password.");
        }



        var domainUser =
            await _userRepository.GetByIdAsync(
                domainUserId,
                cancellationToken);



        if (domainUser is null)
        {
            return Result.Failure(
                "User not found.");
        }



        var identityUser =
            await _userManager.FindByEmailAsync(
                domainUser.Email.Value);



        if (identityUser is null)
        {
            return Result.Failure(
                "Authentication account not found.");
        }



        Result? operationResult = null;



        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            var result =
                await _userManager.ChangePasswordAsync(
                    identityUser,
                    dto.CurrentPassword,
                    dto.NewPassword);



            if (!result.Succeeded)
            {
                operationResult =
                    Result.Failure(
                        result.Errors
                            .Select(e => e.Description));

                return;
            }



            await SyncDomainPasswordHashAsync(
                domainUser,
                dto.NewPassword,
                domainUserId,
                cancellationToken);



            operationResult =
                Result.Success();


        },
        cancellationToken);



        return operationResult!;
    }




    // ============================================================
    // Forgot Password
    // ============================================================

    public async Task<Result> ForgotPasswordAsync(
        ForgotPasswordDto dto,
        string resetPasswordBaseUrl,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            resetPasswordBaseUrl);



        var identityUser =
            await _userManager.FindByEmailAsync(
                dto.Email.Trim().ToLowerInvariant());



        // جلوگیری از Email Enumeration
        if (identityUser is null)
        {
            return Result.Success();
        }



        var rawToken =
            await _userManager.GeneratePasswordResetTokenAsync(
                identityUser);



        var encodedToken =
            WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(rawToken));



        var resetUrl =
            $"{resetPasswordBaseUrl}" +
            $"?email={Uri.EscapeDataString(identityUser.Email!)}" +
            $"&token={encodedToken}";



        var body =
            $"<p>You requested a password reset.</p>" +
            $"<p><a href=\"{resetUrl}\">Reset Password</a></p>" +
            $"<p>If you did not request this, ignore this email.</p>";



        await _emailService.SendEmailAsync(
            identityUser.Email!,
            "Reset Your Password",
            body,
            isHtml: true);



        return Result.Success();
    }




    // ============================================================
    // Reset Password
    // ============================================================

    public async Task<Result> ResetPasswordAsync(
        ResetPasswordDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);



        if (dto.NewPassword != dto.ConfirmPassword)
        {
            return Result.Failure(
                "Passwords do not match.");
        }



        var identityUser =
            await _userManager.FindByEmailAsync(
                dto.Email.Trim().ToLowerInvariant());



        // جلوگیری از Email Enumeration
        if (identityUser is null)
        {
            return Result.Success();
        }



        string rawToken;


        try
        {
            rawToken =
                Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(
                        dto.Token));
        }
        catch (FormatException)
        {
            return Result.Failure(
                "The password reset token is invalid.");
        }



        Result? operationResult = null;



        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            var result =
                await _userManager.ResetPasswordAsync(
                    identityUser,
                    rawToken,
                    dto.NewPassword);



            if (!result.Succeeded)
            {
                operationResult =
                    Result.Failure(
                        result.Errors
                            .Select(e => e.Description));

                return;
            }



            var domainUser =
                await _userRepository.GetByEmailAsync(
                    dto.Email,
                    cancellationToken);



            if (domainUser is not null)
            {
                await SyncDomainPasswordHashAsync(
                    domainUser,
                    dto.NewPassword,
                    domainUser.Id,
                    cancellationToken);
            }



            operationResult =
                Result.Success();


        },
        cancellationToken);



        return operationResult!;
    }

    // ============================================================
    // Admin Reset Password
    // ============================================================

    public async Task<Result> AdminResetPasswordAsync(
        UserResetPasswordDto dto,
        Guid performedBy,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);



        if (dto.NewPassword != dto.ConfirmPassword)
        {
            return Result.Failure(
                "Passwords do not match.");
        }



        var domainUser =
            await _userRepository.GetByIdAsync(
                dto.Id,
                cancellationToken);



        if (domainUser is null)
        {
            return Result.Failure(
                "User not found.");
        }



        var identityUser =
            await _userManager.FindByEmailAsync(
                domainUser.Email.Value);



        if (identityUser is null)
        {
            return Result.Failure(
                "Authentication account not found for this user.");
        }



        Result? operationResult = null;



        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            var removeResult =
                await _userManager.RemovePasswordAsync(
                    identityUser);



            if (!removeResult.Succeeded)
            {
                operationResult =
                    Result.Failure(
                        removeResult.Errors
                            .Select(e => e.Description));

                return;
            }



            var addResult =
                await _userManager.AddPasswordAsync(
                    identityUser,
                    dto.NewPassword);



            if (!addResult.Succeeded)
            {
                operationResult =
                    Result.Failure(
                        addResult.Errors
                            .Select(e => e.Description));

                return;
            }



            await SyncDomainPasswordHashAsync(
                domainUser,
                dto.NewPassword,
                performedBy,
                cancellationToken);



            operationResult =
                Result.Success();


        },
        cancellationToken);



        return operationResult!;
    }




    // ============================================================
    // Confirm Email
    // ============================================================

    public async Task<Result> ConfirmEmailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);



        var identityUser =
            await _userManager.FindByIdAsync(
                userId.ToString());



        if (identityUser is null)
        {
            return Result.Failure(
                "The confirmation link is invalid.");
        }



        if (identityUser.EmailConfirmed)
        {
            return Result.Success();
        }



        string rawToken;



        try
        {
            rawToken =
                Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(
                        token));
        }
        catch (FormatException)
        {
            return Result.Failure(
                "The confirmation link is invalid.");
        }



        var result =
            await _userManager.ConfirmEmailAsync(
                identityUser,
                rawToken);



        if (!result.Succeeded)
        {
            return Result.Failure(
                result.Errors
                    .Select(e => e.Description));
        }



        return Result.Success();
    }




    // ============================================================
    // Resend Confirmation Email
    // ============================================================

    public async Task<Result> ResendConfirmationEmailAsync(
        string email,
        string confirmationBaseUrl,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            confirmationBaseUrl);



        var identityUser =
            await _userManager.FindByEmailAsync(
                email.Trim().ToLowerInvariant());



        // جلوگیری از Email Enumeration
        if (identityUser is null ||
            identityUser.EmailConfirmed)
        {
            return Result.Success();
        }



        var rawToken =
            await _userManager
                .GenerateEmailConfirmationTokenAsync(
                    identityUser);



        var encodedToken =
            WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(rawToken));



        var confirmUrl =
            $"{confirmationBaseUrl}" +
            $"?userId={identityUser.Id}" +
            $"&token={encodedToken}";



        var body =
            $"<p>Please confirm your email address.</p>" +
            $"<p><a href=\"{confirmUrl}\">Confirm Email</a></p>" +
            $"<p>If you did not request this, ignore this email.</p>";



        await _emailService.SendEmailAsync(
            identityUser.Email!,
            "Confirm Your Email",
            body,
            isHtml: true);



        return Result.Success();
    }




    // ============================================================
    // Sync Domain Password Hash
    // ============================================================

    private async Task SyncDomainPasswordHashAsync(
        User domainUser,
        string newPassword,
        Guid? modifiedBy,
        CancellationToken cancellationToken)
    {
        var newHash =
            _passwordHasher.HashPassword(
                newPassword);



        domainUser.ChangePassword(
            newHash,
            modifiedBy);



        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}