using SuperMarket.Application.Common;

namespace SuperMarket.Application.Interfaces.Services;

public interface ICheckoutService
{
    Task<Result<Guid>> CheckoutAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
