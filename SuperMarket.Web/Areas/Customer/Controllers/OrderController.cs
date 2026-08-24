using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.Interfaces.Services;

namespace SuperMarket.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
[Route("orders")]
public sealed class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;
    private readonly ICurrentUserService _currentUser;

    public OrderController(
        IOrderService orderService,
        IPaymentService paymentService,
        ICurrentUserService currentUser)
    {
        _orderService = orderService;
        _paymentService = paymentService;
        _currentUser = currentUser;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.DomainUserId == Guid.Empty)
            return Challenge();

        var result = await _orderService.GetPagedForCustomerAsync(
            _currentUser.DomainUserId,
            pageNumber,
            pageSize,
            cancellationToken);

        return View(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        if (_currentUser.DomainUserId == Guid.Empty)
            return Challenge();

        var result = await _orderService.GetByIdForCustomerAsync(
            id,
            _currentUser.DomainUserId,
            cancellationToken);

        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.FirstError;
            return RedirectToAction(nameof(Index));
        }

        var payments = await _paymentService.GetPagedByOrderIdAsync(
            _currentUser.DomainUserId,
            id,
            1,
            5,
            cancellationToken);

        ViewBag.Payments = payments.IsSuccess ? payments.Value : null;

        return View(result.Value);
    }
}
