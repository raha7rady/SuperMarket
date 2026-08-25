using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.API.Contracts;
using SuperMarket.API.Mapping;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.DTOs.Orders;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Enums;

namespace SuperMarket.API.Controllers;

[ApiController]
[Route("api/checkout")]
[Authorize]
public sealed class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;
    private readonly IOrderService _orderService;
    private readonly ICurrentUserService _currentUser;

    public CheckoutController(
        ICheckoutService checkoutService,
        IOrderService orderService,
        ICurrentUserService currentUser)
    {
        _checkoutService = checkoutService ?? throw new ArgumentNullException(nameof(checkoutService));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest? request, CancellationToken cancellationToken)
    {
        var checkoutResult = request is null
            ? await _checkoutService.CheckoutAsync(_currentUser.DomainUserId, cancellationToken)
            : await _checkoutService.CheckoutAsync(_currentUser.DomainUserId, ToDetailsDto(request), cancellationToken);

        if (checkoutResult.IsFailure)
        {
            return BadRequest(new { errors = checkoutResult.Errors });
        }

        var orderResult = await _orderService.GetByIdForCustomerAsync(
            checkoutResult.Value,
            _currentUser.DomainUserId,
            cancellationToken);

        if (orderResult.IsFailure)
        {
            return Ok(new { orderId = checkoutResult.Value });
        }

        return Ok(orderResult.Value.ToResponse());
    }

    private static CheckoutDetailsDto ToDetailsDto(CheckoutRequest request)
    {
        return new CheckoutDetailsDto
        {
            RecipientFullName = request.RecipientFullName,
            RecipientPhone = request.RecipientPhone,
            Province = request.Province,
            City = request.City,
            AddressLine = request.AddressLine,
            PostalCode = request.PostalCode,
            Plaque = request.Plaque,
            Unit = request.Unit,
            DeliveryNote = request.DeliveryNote,
            DeliveryOption = MapDeliveryOption(request.DeliveryOption),
            PaymentMethod = MapPaymentMethod(request.PaymentMethod),
            ShippingCost = request.ShippingCost,
            CouponCode = request.CouponCode,
            CouponDiscount = request.CouponDiscount
        };
    }

    private static DeliveryOption MapDeliveryOption(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            "evening" => DeliveryOption.Evening,
            "tomorrow" => DeliveryOption.Tomorrow,
            _ => DeliveryOption.Express
        };
    }

    private static OrderPaymentMethod MapPaymentMethod(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            "online" => OrderPaymentMethod.Online,
            "wallet" => OrderPaymentMethod.Wallet,
            "cod" => OrderPaymentMethod.Cod,
            _ => OrderPaymentMethod.Ipg
        };
    }
}
