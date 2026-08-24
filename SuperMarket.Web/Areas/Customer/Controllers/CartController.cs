using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Common;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.DTOs.Cart;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Web.Areas.Customer.ViewModels.Cart;


namespace SuperMarket.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
[AutoValidateAntiforgeryToken]
[Route("cart")]
public sealed class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly ILogger<CartController> _logger;

    public CartController(
        ICartService cartService,
        ICurrentUserService currentUser,
        IMapper mapper,
        ILogger<CartController> logger)
    {
        _cartService = cartService;
        _currentUser = currentUser;
        _mapper = mapper;
        _logger = logger;
    }

    #region Index

    [HttpGet("")]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        if (_currentUser.DomainUserId == Guid.Empty)
            return Challenge();

        try
        {
            var result = await _cartService.GetByUserIdAsync(
    _currentUser.DomainUserId,
    cancellationToken);

            if (result.IsFailure)
            {
                return View(new CartIndexViewModel
                {
                    Cart = new CartViewModel()
                });
            }

            return View(new CartIndexViewModel
            {
                Cart = _mapper.Map<CartViewModel>(result.Value)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error loading cart. UserId:{UserId}",
                _currentUser.UserId);

            TempData["ErrorMessage"] =
                "خطا در دریافت اطلاعات سبد خرید.";

            return View(new CartIndexViewModel
            {
                Cart = new CartViewModel()
            });
        }
    }

    #endregion





    #region Add Item

    [HttpPost("add")]
    public async Task<IActionResult> Add(
        AddToCartViewModel model,
        CancellationToken cancellationToken)
    {
        if (_currentUser.DomainUserId == Guid.Empty)
            return Challenge();

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] =
                "اطلاعات وارد شده معتبر نیست.";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            var cartResult =
                await EnsureCartAsync(cancellationToken);

            if (cartResult.IsFailure)
            {
                TempData["ErrorMessage"] =
                    cartResult.FirstError;

                return RedirectToAction(nameof(Index));
            }

            var result = await _cartService.AddItemAsync(
                cartResult.Value,
                _mapper.Map<CartItemDto>(model),
                cancellationToken);

            TempData[result.IsSuccess
                ? "SuccessMessage"
                : "ErrorMessage"] =
                result.IsSuccess
                    ? "کالا به سبد خرید اضافه شد."
                    : result.FirstError;

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Add to cart error. ProductId:{ProductId}, UserId:{UserId}",
                model.ProductId,
                _currentUser.DomainUserId);

            TempData["ErrorMessage"] =
                "خطا در افزودن کالا به سبد خرید.";

            return RedirectToAction(nameof(Index));
        }
    }

    #endregion

    #region Update Item

    [HttpPost("update")]
    public async Task<IActionResult> Update(
        UpdateCartItemViewModel model,
        CancellationToken cancellationToken)
    {
        if (_currentUser.DomainUserId == Guid.Empty)
            return Challenge();

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] =
                "اطلاعات وارد شده معتبر نیست.";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            var cartResult =
                await EnsureCartAsync(cancellationToken);

            if (cartResult.IsFailure)
            {
                TempData["ErrorMessage"] =
                    cartResult.FirstError;

                return RedirectToAction(nameof(Index));
            }

            var result = await _cartService.UpdateItemAsync(
                cartResult.Value,
                _mapper.Map<CartUpdateItemDto>(model),
                cancellationToken);

            TempData[result.IsSuccess
                ? "SuccessMessage"
                : "ErrorMessage"] =
                result.IsSuccess
                    ? "سبد خرید بروزرسانی شد."
                    : result.FirstError;

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Update cart error. ProductId:{ProductId}, UserId:{UserId}",
                model.ProductId,
                _currentUser.DomainUserId);

            TempData["ErrorMessage"] =
                "خطا در بروزرسانی سبد خرید.";

            return RedirectToAction(nameof(Index));
        }
    }

    #endregion

    #region Remove Item

    [HttpPost("remove/{productId:guid}")]
    public async Task<IActionResult> Remove(
        Guid productId,
        CancellationToken cancellationToken)
    {
        if (_currentUser.DomainUserId == Guid.Empty)
            return Challenge();

        try
        {
            var cartResult =
                await EnsureCartAsync(cancellationToken);

            if (cartResult.IsFailure)
            {
                TempData["ErrorMessage"] =
                    cartResult.FirstError;

                return RedirectToAction(nameof(Index));
            }

            var result = await _cartService.RemoveItemAsync(
    cartResult.Value,
    productId,
    _currentUser.DomainUserId,
    cancellationToken);

            TempData[result.IsSuccess
                ? "SuccessMessage"
                : "ErrorMessage"] =
                result.IsSuccess
                    ? "کالا حذف شد."
                    : result.FirstError;

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Remove cart item error. ProductId:{ProductId}, UserId:{UserId}",
                productId,
                _currentUser.UserId);

            TempData["ErrorMessage"] =
                "خطا در حذف کالا.";

            return RedirectToAction(nameof(Index));
        }
    }

    #endregion

    #region Clear Cart

    [HttpPost("clear")]
    public async Task<IActionResult> Clear(
        CancellationToken cancellationToken)
    {
        if (_currentUser.DomainUserId == Guid.Empty)
            return Challenge();

        try
        {
            var cartResult =
                await EnsureCartAsync(cancellationToken);

            if (cartResult.IsFailure)
            {
                TempData["ErrorMessage"] =
                    cartResult.FirstError;

                return RedirectToAction(nameof(Index));
            }

            var result = await _cartService.ClearAsync(
     cartResult.Value,
     _currentUser.DomainUserId,
     cancellationToken);

            TempData[result.IsSuccess
                ? "SuccessMessage"
                : "ErrorMessage"] =
                result.IsSuccess
                    ? "سبد خرید خالی شد."
                    : result.FirstError;

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Clear cart error. UserId:{UserId}",
                _currentUser.UserId);

            TempData["ErrorMessage"] =
                "خطا در پاکسازی سبد خرید.";

            return RedirectToAction(nameof(Index));
        }
    }

    #endregion

    #region Helpers

    private async Task<Result<Guid>> EnsureCartAsync(
        CancellationToken cancellationToken)
    {
        var cart = await _cartService.GetByUserIdAsync(
            _currentUser.DomainUserId,
            cancellationToken);

        if (cart.IsSuccess)
            return Result<Guid>.Success(cart.Value.Id);

        return await _cartService.CreateAsync(
            new CartCreateDto
            {
                UserId = _currentUser.DomainUserId
            },
            cancellationToken);
    }

    #endregion
}