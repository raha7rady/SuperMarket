using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Orders;
using SuperMarket.Application.DTOs.Payments;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Interfaces.Repositories;

namespace SuperMarket.Application.Services;

public sealed class CheckoutService : ICheckoutService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutService(
        ICartRepository cartRepository,
        ICartService cartService,
        IOrderService orderService,
        IPaymentService paymentService,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _cartService = cartService;
        _orderService = orderService;
        _paymentService = paymentService;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> CheckoutAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return Result<Guid>.Failure("UserId is required.");

        Result<Guid>? result = null;

        try
        {
            await _unitOfWork.ExecuteTransactionAsync(async () =>
            {
                // Blocks concurrent checkouts for the same user's cart.
                await _cartRepository.LockForCheckoutAsync(userId, cancellationToken);

                var cart = await _cartRepository.GetActiveCartWithItemsByUserIdAsync(userId, cancellationToken);

                if (cart is null || !cart.HasItems)
                {
                    result = Result<Guid>.Failure("Cart is empty.");
                    return;
                }

                var items = cart.Items.Where(i => !i.IsDeleted).ToList();
                var totalAmount = 0m;

                foreach (var item in items)
                {
                    var product = await _productRepository.GetByIdWithCategoryAsync(item.ProductId, cancellationToken);

                    if (product is null || !product.IsActive)
                        throw new InvalidOperationException($"Product {item.ProductId} is not available.");

                    var decreased = await _productRepository.TryDecreaseStockAsync(
                        item.ProductId,
                        item.Quantity.Value,
                        cancellationToken);

                    if (!decreased)
                        throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}.");

                    totalAmount += product.Price.Amount * item.Quantity.Value;
                }

                var orderDto = new CreateOrderDto
                {
                    UserId = userId,
                    Items = items
                        .Select(i => new OrderItemDto { ProductId = i.ProductId, Quantity = i.Quantity.Value })
                        .ToList()
                };

                var orderResult = await _orderService.CreateAsync(orderDto, cancellationToken);

                if (orderResult.IsFailure)
                    throw new InvalidOperationException(orderResult.FirstError);

                // Prepares the order for a future real payment gateway; no charge happens here.
                var paymentResult = await _paymentService.CreateAsync(
                    new PaymentCreateDto
                    {
                        OrderId = orderResult.Value,
                        Amount = totalAmount,
                        PaymentMethod = "Unspecified",
                        TransactionId = string.Empty,
                        Description = "Created at checkout"
                    },
                    cancellationToken);

                if (paymentResult.IsFailure)
                    throw new InvalidOperationException(paymentResult.FirstError);

                var clearResult = await _cartService.ClearAsync(cart.Id, userId, cancellationToken);

                if (clearResult.IsFailure)
                    throw new InvalidOperationException(clearResult.FirstError);

                result = orderResult;
            },
            cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }

        return result ?? Result<Guid>.Failure("Checkout failed.");
    }
}
