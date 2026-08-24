
using Microsoft.AspNetCore.Identity;
using SuperMarket.Application.Common.Interfaces;

namespace SuperMarket.Infrastructure.Services;
/// <summary>
/// Wraps ASP.NET Core Identity's <see cref="Microsoft.AspNetCore.Identity.PasswordHasher{TUser}"/>
/// so the Domain's PasswordHash value object can be produced/verified
/// without a dependency on Identity's ApplicationUser type.
/// NOTE: PasswordHasher&lt;TUser&gt; does not use the TUser instance for
/// hashing/verification (only the password string), so passing a throwaway
/// <see cref="object"/> here is safe with the default implementation. This is
/// an intentional, documented reliance on that (stable, well-known) behavior —
/// not an oversight.
/// </summary>

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<object> _hasher;

    public PasswordHasher()
    {
        _hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
    }

    public string HashPassword(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Password cannot be empty.", nameof(plainPassword));

        return _hasher.HashPassword(new object(), plainPassword);
    }

    public bool VerifyHashedPassword(string plainPassword, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            return false;

        if (string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        var result = _hasher.VerifyHashedPassword(new object(), hashedPassword, plainPassword);

        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}

