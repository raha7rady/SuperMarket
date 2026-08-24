using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Common;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.Interfaces.Services;
using DomainPayment = SuperMarket.Domain.Enums.PaymentStatus;

namespace SuperMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SuperAdmin")]
public sealed class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly ICurrentUserService _currentUser;

    public PaymentsController(IPaymentService paymentService, ICurrentUserService currentUser)
    {
        _paymentService = paymentService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _paymentService.GetPagedForAdminAsync(pageNumber, pageSize, cancellationToken);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var result = await _paymentService.GetByIdForAdminAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            TempData["Error"] = result.FirstError;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> MarkAsProcessing(Guid id, CancellationToken ct) => ChangeStatus(id, DomainPayment.Processing, ct);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> MarkAsPaid(Guid id, CancellationToken ct) => ChangeStatus(id, DomainPayment.Paid, ct);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> MarkAsFailed(Guid id, CancellationToken ct) => ChangeStatus(id, DomainPayment.Failed, ct);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> MarkAsRefunded(Guid id, CancellationToken ct) => ChangeStatus(id, DomainPayment.Refunded, ct);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Cancel(Guid id, CancellationToken ct) => ChangeStatus(id, DomainPayment.Canceled, ct);

    private async Task<IActionResult> ChangeStatus(Guid id, DomainPayment status, CancellationToken ct)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var userId = _currentUser.DomainUserId;
        if (userId == Guid.Empty)
            return Unauthorized();

        var result = await _paymentService.ChangeStatusAsync(id, status, userId, ct);

        TempData[result.IsFailure ? "Error" : "Success"] = result.IsFailure ? result.FirstError : "Payment status updated.";

        return RedirectToAction(nameof(Details), new { id });
    }
}
