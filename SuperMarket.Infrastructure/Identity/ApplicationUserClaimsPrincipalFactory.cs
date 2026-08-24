using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace SuperMarket.Infrastructure.Identity;

/// <summary>
/// Ensures every sign-in principal carries the DomainUserId claim, so
/// ICurrentUserService can resolve it regardless of which SignInManager
/// method triggered the sign-in.
/// </summary>
public sealed class ApplicationUserClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole<Guid>>
{
    public ApplicationUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options)
    {
    }

    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        var identity = (ClaimsIdentity)principal.Identity!;

        identity.AddClaim(new Claim(IdentityClaimNames.DomainUserId, user.DomainUserId.ToString()));

        return principal;
    }
}
