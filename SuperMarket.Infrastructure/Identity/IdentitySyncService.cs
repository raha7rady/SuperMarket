using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuperMarket.Application.Common;
using SuperMarket.Application.Common.Extensions;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Domain.Enums;

namespace SuperMarket.Infrastructure.Identity;

/// <summary>
/// Infrastructure-side implementation of <see cref="IIdentitySyncService"/>.
/// Wraps <see cref="UserManager{TUser}"/> so the Application layer can keep
/// ASP.NET Core Identity in sync with the Domain <c>User</c> without taking
/// a direct dependency on Identity types (matches the pattern already used
/// by <c>AccountService</c> and <c>IdentitySeeder</c>).
/// </summary>
public sealed class IdentitySyncService : IIdentitySyncService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentitySyncService(UserManager<ApplicationUser> userManager)
    {
        ArgumentNullException.ThrowIfNull(userManager);

        _userManager = userManager;
    }

    public async Task<Result> CreateIdentityUserAsync(
            Guid domainUserId,
            string email,
            string password,
            UserRole role,
            CancellationToken cancellationToken = default)
    {
        if (domainUserId == Guid.Empty)
            throw new ArgumentException("Domain user id cannot be empty.", nameof(domainUserId));

        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var existing = await FindByDomainUserIdAsync(domainUserId, cancellationToken);

        if (existing is not null)
        {
            return Result.Failure("An identity account already exists for this user.");
        }

        var identityUser = ApplicationUser.Create(domainUserId, email);

        var createResult = await _userManager.CreateAsync(identityUser, password);

        if (!createResult.Succeeded)
        {
            return Result.Failure(
                createResult.Errors.Select(e => e.Description));
        }

        var roleResult = await _userManager.AddToRoleAsync(
            identityUser,
            role.ToRoleName());

        if (!roleResult.Succeeded)
        {
            // Compensating action: بدون این، در صورتی که این متد خارج از
            // یک Transaction فراخوانی شود (یا Rollback فراخواننده به هر
            // دلیلی اجرا نشود)، یک کاربر Identity ناقص (بدون نقش) باقی
            // می‌ماند. با حذف صریح، وضعیت به حالت قبل از فراخوانی برمی‌گردد.
            await _userManager.DeleteAsync(identityUser);

            return Result.Failure(
                roleResult.Errors.Select(e => e.Description));
        }

        return Result.Success();
    }

    public async Task<Result> SyncRoleAsync(
        Guid domainUserId,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        var identityUser = await FindByDomainUserIdAsync(domainUserId, cancellationToken);

        if (identityUser is null)
        {
            return Result.Failure("No identity account found for this user.");
        }

        var currentRoles = await _userManager.GetRolesAsync(identityUser);

        var targetRole = role.ToRoleName();

        var rolesToRemove = currentRoles
            .Where(r => !string.Equals(r, targetRole, StringComparison.Ordinal))
            .ToList();

        if (rolesToRemove.Count > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(identityUser, rolesToRemove);

            if (!removeResult.Succeeded)
            {
                return Result.Failure(
                    removeResult.Errors.Select(e => e.Description));
            }
        }

        if (!currentRoles.Contains(targetRole, StringComparer.Ordinal))
        {
            var addResult = await _userManager.AddToRoleAsync(identityUser, targetRole);

            if (!addResult.Succeeded)
            {
                return Result.Failure(
                    addResult.Errors.Select(e => e.Description));
            }
        }

        return Result.Success();
    }

    public async Task<Result> SyncEmailAsync(
            Guid domainUserId,
            string newEmail,
            CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newEmail);

        var identityUser = await FindByDomainUserIdAsync(domainUserId, cancellationToken);

        if (identityUser is null)
        {
            return Result.Failure("No identity account found for this user.");
        }

        identityUser.ChangeEmail(newEmail);

        // آدرس ایمیل جدید هنوز توسط مالک واقعی تأیید نشده است؛ اگر
        // EmailConfirmed از مقدار قبلی (احتمالاً true) دست‌نخورده بماند،
        // یک ایمیل تأییدنشده به‌اشتباه «تأییدشده» تلقی می‌شود.
        identityUser.EmailConfirmed = false;

        var updateResult = await _userManager.UpdateAsync(identityUser);

        if (!updateResult.Succeeded)
        {
            return Result.Failure(
                updateResult.Errors.Select(e => e.Description));
        }

        return Result.Success();
    }

    private Task<ApplicationUser?> FindByDomainUserIdAsync(
        Guid domainUserId,
        CancellationToken cancellationToken)
    {
        return _userManager.Users
            .FirstOrDefaultAsync(u => u.DomainUserId == domainUserId, cancellationToken);
    }
}