using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.Interfaces.Services;

namespace SuperMarket.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
[AutoValidateAntiforgeryToken]
[Route("checkout")]
public sealed class CheckoutController : Controller
{
    private readonly ICheckoutService _checkoutService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CheckoutController> _logger;

    public CheckoutController(
        ICheckoutService checkoutService,
        ICurrentUserService currentUser,
        ILogger<CheckoutController> logger)
    {
        _checkoutService = checkoutService;
        _currentUser = currentUser;
        _logger = logger;
    }

    [HttpPost("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (_currentUser.DomainUserId == Guid.Empty)
            return Challenge();

        try
        {
            var result = await _checkoutService.CheckoutAsync(
                _currentUser.DomainUserId,
                cancellationToken);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"سفارش شما با موفقیت ثبت شد. کد سفارش: {result.Value}";
                return RedirectToAction("Details", "Order", new { area = "Customer", id = result.Value });
            }

            TempData["ErrorMessage"] = result.FirstError;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Checkout error. UserId:{UserId}",
                _currentUser.DomainUserId);

            TempData["ErrorMessage"] = "خطا در ثبت سفارش.";
        }

        return RedirectToAction("Index", "Cart");
    }
}
