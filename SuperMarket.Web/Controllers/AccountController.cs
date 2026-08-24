using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.DTOs.Account;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Web.ViewModels;

namespace SuperMarket.Web.Controllers;

public sealed class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly ICurrentUserService _currentUserService;

    public AccountController(
        IAccountService accountService,
        ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(accountService);
        ArgumentNullException.ThrowIfNull(currentUserService);

        _accountService = accountService;
        _currentUserService = currentUserService;
    }

    #region Register

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        RegisterViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _accountService.RegisterAsync(
                new RegisterDto
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword
                },
                cancellationToken);

        if (result.IsFailure)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error);
            }

            return View(model);
        }

        TempData["SuccessMessage"] = result.Value.RequiresEmailConfirmation
            ? "Registration completed successfully. Please check your email to confirm your account."
            : "Registration completed successfully.";

        return RedirectToAction(
            nameof(Login));
    }

    #endregion

    #region Login

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(
        string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _accountService.LoginAsync(
                new LoginDto
                {
                    Email = model.Email,
                    Password = model.Password,
                    RememberMe = model.RememberMe
                },
                cancellationToken);

        if (result.IsFailure)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error);
            }

            return View(model);
        }

        if (result.Value.IsLockedOut)
        {
            ModelState.AddModelError(
                string.Empty,
                "Your account has been locked.");

            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) &&
            Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(
            "Index",
            "Home");
    }

    #endregion

    #region Logout

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();

        return RedirectToAction(
            nameof(Login));
    }

    #endregion

    #region AccessDenied

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied(
        string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        return View();
    }

    #endregion

    #region Forgot Password

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _accountService.ForgotPasswordAsync(
                new ForgotPasswordDto
                {
                    Email = model.Email
                },
                Url.Action(
                    nameof(ResetPassword),
                    "Account",
                    null,
                    Request.Scheme)!,
                cancellationToken);

        if (result.IsFailure)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error);
            }

            return View(model);
        }

        TempData["SuccessMessage"] =
            "If an account with this email exists, a password reset link has been sent.";

        return RedirectToAction(
            nameof(Login));
    }

    #endregion

    #region Reset Password

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResetPassword(
        string email,
        string token)
    {
        return View(
            new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _accountService.ResetPasswordAsync(
                new ResetPasswordDto
                {
                    Email = model.Email,
                    Token = model.Token,
                    NewPassword = model.NewPassword,
                    ConfirmPassword = model.ConfirmPassword
                },
                cancellationToken);

        if (result.IsFailure)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error);
            }

            return View(model);
        }

        TempData["SuccessMessage"] =
            "Your password has been reset successfully.";

        return RedirectToAction(
            nameof(Login));
    }

    #endregion

    #region Confirm Email

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _accountService.ConfirmEmailAsync(
                userId,
                token,
                cancellationToken);

        if (result.IsFailure)
        {
            TempData["ErrorMessage"] =
                string.Join(
                    Environment.NewLine,
                    result.Errors);

            return RedirectToAction(
                nameof(Login));
        }

        TempData["SuccessMessage"] =
            "Your email has been confirmed successfully.";

        return RedirectToAction(
            nameof(Login));
    }

    #endregion

    #region Resend Confirmation Email

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResendConfirmationEmail()
    {
        return View(new ResendConfirmationEmailViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendConfirmationEmail(
        ResendConfirmationEmailViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _accountService.ResendConfirmationEmailAsync(
                model.Email,
                Url.Action(
                    nameof(ConfirmEmail),
                    "Account",
                    null,
                    Request.Scheme)!,
                cancellationToken);

        if (result.IsFailure)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error);
            }

            return View(model);
        }

        TempData["SuccessMessage"] =
            "A new confirmation email has been sent if the account exists.";

        return RedirectToAction(
            nameof(Login));
    }

    #endregion

    #region Change Password

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordViewModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _accountService.ChangePasswordAsync(
                new ChangePasswordDto
                {
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword,
                    ConfirmPassword = model.ConfirmPassword
                },
                _currentUserService.DomainUserId,
                cancellationToken);

        if (result.IsFailure)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error);
            }

            return View(model);
        }

        TempData["SuccessMessage"] =
            "Your password has been changed successfully.";

        return RedirectToAction(
            nameof(Login));
    }

    #endregion
}