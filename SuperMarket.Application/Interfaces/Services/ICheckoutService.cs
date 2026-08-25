using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Orders;

namespace SuperMarket.Application.Interfaces.Services;

public interface ICheckoutService
{
    Task<Result<Guid>> CheckoutAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> CheckoutAsync(
        Guid userId,
        CheckoutDetailsDto details,
        CancellationToken cancellationToken = default);
}
