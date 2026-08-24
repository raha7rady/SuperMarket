using SuperMarket.Application.Common;
using SuperMarket.Domain.Enums;

namespace SuperMarket.Application.Common.Interfaces;

/// <summary>
/// Keeps ASP.NET Core Identity's user record (email/username and role
/// membership) in sync with the corresponding Domain <c>User</c> whenever
/// they change outside of registration — e.g. an admin editing a user's
/// email or role. Implemented in Infrastructure, where UserManager lives;
/// Application depends only on this abstraction, so the dependency
/// direction required by Clean Architecture is preserved.
/// </summary>
public interface IIdentitySyncService
{
    /// <summary>
    /// Creates the ASP.NET Core Identity user linked to
    /// <paramref name="domainUserId"/> and assigns it to <paramref name="role"/>.
    /// Used whenever a Domain <c>User</c> is created outside of the
    /// self-service registration flow (e.g. an admin creating a user via
    /// <c>UserService.CreateAsync</c>), so the Identity store is no longer
    /// left out of sync.
    /// </summary>
    Task<Result> CreateIdentityUserAsync(
        Guid domainUserId,
        string email,
        string password,
        UserRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures the Identity user linked to <paramref name="domainUserId"/>
    /// is a member of exactly the role matching <paramref name="role"/>,
    /// removing any other role memberships it currently has.
    /// </summary>
    Task<Result> SyncRoleAsync(
        Guid domainUserId,
        UserRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the Identity user's email and username to match
    /// <paramref name="newEmail"/> so the user can continue to sign in
    /// with their new email address.
    /// </summary>
    Task<Result> SyncEmailAsync(
        Guid domainUserId,
        string newEmail,
        CancellationToken cancellationToken = default);
}