using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Infrastructure.Identity;

namespace SuperMarket.Infrastructure.Services;

public sealed class CurrentUserService
    : ICurrentUserService
{
    private readonly IHttpContextAccessor
        _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor =
            httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor
            .HttpContext?
            .User;

    public Guid UserId
    {
        get
        {
            if (!IsAuthenticated)
                return Guid.Empty;

            var value =
                User?.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return Guid.TryParse(
                value,
                out var id)
                ? id
                : Guid.Empty;
        }
    }

    public Guid DomainUserId
    {
        get
        {
            if (!IsAuthenticated)
                return Guid.Empty;

            var value =
                User?.FindFirstValue(
                IdentityClaimNames.DomainUserId);

            return Guid.TryParse(
                value,
                out var id)
                ? id
                : Guid.Empty;
        }
    }

    public string? Email =>
        User?.FindFirstValue(
            ClaimTypes.Email);

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated
        ?? false;
}