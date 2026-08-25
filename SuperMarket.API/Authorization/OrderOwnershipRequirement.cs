using Microsoft.AspNetCore.Authorization;

namespace SuperMarket.API.Authorization;

public sealed class OrderOwnershipRequirement : IAuthorizationRequirement
{
}
