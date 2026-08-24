
using Microsoft.AspNetCore.Identity;

namespace SuperMarket.Infrastructure.Identity;
/// <summary>
/// ASP.NET Core Identity user — the single source of truth for
/// authentication (password hash, lockout, security stamp, roles).
/// <see cref="DomainUserId"/> links this record to the corresponding
/// <c>SuperMarket.Domain.Entities.User</c>, which holds application-level
/// data (name, role for domain rules, orders, carts). Keeping both records
/// in sync (email, role, password) is the responsibility of
/// <c>AccountService</c> / <c>UserService</c> — never update one store
/// without the other.
/// </summary>

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public Guid DomainUserId { get; private set; }

    private ApplicationUser()
    {
        // Required by EF Core
    }

    private ApplicationUser(Guid domainUserId, string email)
    {
        if (domainUserId == Guid.Empty)
            throw new ArgumentException("DomainUserId cannot be empty.", nameof(domainUserId));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        email = email.Trim().ToLowerInvariant();

        Id = Guid.NewGuid();
        DomainUserId = domainUserId;

        UserName = email;
        Email = email;
        NormalizedUserName = email.ToUpperInvariant();
        NormalizedEmail = email.ToUpperInvariant();

        EmailConfirmed = false;
        SecurityStamp = Guid.NewGuid().ToString();
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }

    public static ApplicationUser Create(Guid domainUserId, string email)
        => new(domainUserId, email);

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        SecurityStamp = Guid.NewGuid().ToString();
    }

    public void ChangeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        email = email.Trim().ToLowerInvariant();

        UserName = email;
        Email = email;
        NormalizedUserName = email.ToUpperInvariant();
        NormalizedEmail = email.ToUpperInvariant();

        SecurityStamp = Guid.NewGuid().ToString();
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }
}

