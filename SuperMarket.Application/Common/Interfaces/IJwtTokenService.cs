using System;
using System.Collections.Generic;

namespace SuperMarket.Application.Common.Interfaces;

public sealed record JwtToken(string AccessToken, DateTimeOffset ExpiresAtUtc);

public interface IJwtTokenService
{
    JwtToken GenerateToken(
        Guid identityUserId,
        Guid domainUserId,
        string email,
        IEnumerable<string> roles);
}
