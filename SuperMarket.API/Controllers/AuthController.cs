using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Contracts;
using SuperMarket.Application.Common.Extensions;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.DTOs.Account;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Enums;
using SuperMarket.Infrastructure.Identity;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICurrentUserService _currentUser;

    public AuthController(
        IAccountService accountService,
        IUserService userService,
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        ICurrentUserService currentUser)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        var registerResult = await _accountService.RegisterAsync(dto, cancellationToken);

        if (registerResult.IsFailure)
        {
            return BadRequest(new { errors = registerResult.Errors });
        }

        var result = registerResult.Value;

        var response = await BuildAuthResponseAsync(
            result.Email,
            result.UserId,
            result.FullName,
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var loginResult = await _accountService.LoginAsync(dto, cancellationToken);

        if (loginResult.IsFailure)
        {
            return Unauthorized(new { errors = loginResult.Errors });
        }

        var result = loginResult.Value;

        if (result.IsLockedOut)
        {
            return Unauthorized(new { errors = new[] { "Account is locked out." } });
        }

        if (result.RequiresTwoFactor)
        {
            return Unauthorized(new { errors = new[] { "Two-factor authentication is required." } });
        }

        var response = await BuildAuthResponseAsync(
            result.Email,
            result.UserId,
            result.FullName,
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var profileResult = await _userService.GetByIdForCustomerAsync(_currentUser.DomainUserId, cancellationToken);

        return Ok(new CurrentUserResponse
        {
            UserId = _currentUser.DomainUserId,
            Email = _currentUser.Email,
            FullName = profileResult.IsSuccess ? profileResult.Value.FullName : string.Empty,
            IsAuthenticated = _currentUser.IsAuthenticated
        });
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(
        string email,
        Guid domainUserId,
        string fullName,
        CancellationToken cancellationToken)
    {
        var identityUser = await _userManager.FindByEmailAsync(email);

        if (identityUser is null)
        {
            throw new InvalidOperationException("Identity user not found after a successful account operation.");
        }

        var roles = await _userManager.GetRolesAsync(identityUser);

        var token = _jwtTokenService.GenerateToken(
            identityUser.Id,
            domainUserId,
            email,
            roles);

        return new AuthResponse
        {
            AccessToken = token.AccessToken,
            ExpiresAtUtc = token.ExpiresAtUtc,
            UserId = domainUserId,
            Email = email,
            FullName = fullName,
            Role = roles.FirstOrDefault() ?? UserRole.Customer.ToRoleName()
        };
    }
}
