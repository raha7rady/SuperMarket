using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Common;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Web.Areas.Admin.Mappings;
using SuperMarket.Web.Areas.Admin.ViewModels.Orders;
using SuperMarket.Web.Mapping;

namespace SuperMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SuperAdmin")]
public sealed class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICurrentUserService _currentUser;

    public OrdersController(IOrderService orderService, ICurrentUserService currentUser)
    {
        _orderService = orderService;
        _currentUser = currentUser;
    }

    #region Utilities

    private bool TryGetCurrentUserId(out Guid userId)
    {
        userId = _currentUser.DomainUserId;
        return userId != Guid.Empty;
    }

    private IActionResult RedirectWithMessage(
        Result result,
        string successMessage,
        string actionName,
        Guid? id = null)
    {
        if (result.IsFailure)
        {
            TempData["Error"] = result.FirstError;
        }
        else
        {
            TempData["Success"] = successMessage;
        }

        return id.HasValue
            ? RedirectToAction(actionName, new { id })
            : RedirectToAction(actionName);
    }

    #endregion

    // ============================================================
    // LIST
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Index(
        OrderFilterViewModel filter,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(new OrderListViewModel { Filter = filter });

        var result = await _orderService.GetPagedForAdminAsync(
            filter.PageNumber,
            filter.PageSize,
            cancellationToken);

        if (result.IsFailure)
        {
            TempData["Error"] = result.FirstError;
            return View(new OrderListViewModel { Filter = filter });
        }

        var items = result.Value
            .Select(OrderMappings.ToListItemViewModel)
            .ToList();

        var viewModel = new OrderListViewModel
        {
            Items = items,
            Filter = filter,
            TotalCount = result.TotalCount
        };

        return View(viewModel);
    }

    // ============================================================
    // DETAILS
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Details(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            return BadRequest();

        var result = await _orderService.GetByIdForAdminAsync(
            id,
            cancellationToken);

        if (result.IsFailure)
        {
            TempData["Error"] = result.FirstError;
            return RedirectToAction(nameof(Index));
        }

        var viewModel = OrderMappings.ToDetailsViewModel(result.Value);

        return View(viewModel);
    }

    // ============================================================
    // STATUS OPERATIONS
    // ============================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPaid(Guid id, CancellationToken ct)
        => await ExecuteStatusChange(id, _orderService.MarkAsPaidAsync,
            "Order marked as paid.", ct);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsShipped(Guid id, CancellationToken ct)
        => await ExecuteStatusChange(id, _orderService.MarkAsShippedAsync,
            "Order marked as shipped.", ct);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsDelivered(Guid id, CancellationToken ct)
        => await ExecuteStatusChange(id, _orderService.MarkAsDeliveredAsync,
            "Order marked as delivered.", ct);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        => await ExecuteStatusChange(id, _orderService.CancelAsync,
            "Order cancelled.", ct);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRefunded(Guid id, CancellationToken ct)
        => await ExecuteStatusChange(id, _orderService.MarkAsRefundedAsync,
            "Order refunded.", ct);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
        => await ExecuteStatusChange(id, _orderService.RestoreAsync,
            "Order restored.", ct);

    private async Task<IActionResult> ExecuteStatusChange(
        Guid id,
        Func<Guid, Guid, CancellationToken, Task<Result>> operation,
        string successMessage,
        CancellationToken ct)
    {
        if (id == Guid.Empty)
            return BadRequest();

        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized();

        var result = await operation(id, userId, ct);

        return RedirectWithMessage(result, successMessage, nameof(Details), id);
    }

    // ============================================================
    // DELETE
    // ============================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (id == Guid.Empty)
            return BadRequest();

        if (!TryGetCurrentUserId(out var userId))
            return Unauthorized();

        var result = await _orderService.DeleteAsync(id, userId, ct);

        return RedirectWithMessage(result, "Order deleted.", nameof(Index));
    }
}