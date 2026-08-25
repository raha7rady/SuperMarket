using Microsoft.AspNetCore.Authorization;
using SuperMarket.Application.Common.Interfaces;

namespace SuperMarket.API.Authorization;

public sealed class OrderOwnershipHandler : AuthorizationHandler<OrderOwnershipRequirement, Guid>
{
    private readonly ICurrentUserService _currentUser;

    public OrderOwnershipHandler(ICurrentUserService currentUser)
    {
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OrderOwnershipRequirement requirement,
        Guid resource)
    {
        if (context.User.IsInRole("Admin") || context.User.IsInRole("SuperAdmin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (_currentUser.IsAuthenticated && _currentUser.DomainUserId == resource)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
