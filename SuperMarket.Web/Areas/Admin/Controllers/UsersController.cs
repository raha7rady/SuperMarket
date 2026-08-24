using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Common;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Application.DTOs.Users;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Enums;
using SuperMarket.Web.Areas.Admin.ViewModels.Users;
using SuperMarket.Web.Mapping;

namespace SuperMarket.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public sealed class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly ICurrentUserService _currentUser;
        public UsersController(
            IUserService userService,
            IAccountService accountService,
            ICurrentUserService currentUser)
        {
            ArgumentNullException.ThrowIfNull(userService);
            ArgumentNullException.ThrowIfNull(accountService);
            ArgumentNullException.ThrowIfNull(currentUser);

            _userService = userService;
            _accountService = accountService;
            _currentUser = currentUser;
        }

        #region Utilities

        private bool TryGetCurrentUserId(out Guid userId)
        {
            userId = _currentUser.DomainUserId;
            return userId != Guid.Empty;
        }

        private IActionResult RedirectWithMessage(Result result, string successMessage, string actionName, Guid? id = null)
        {
            if (result.IsFailure)
                TempData["Error"] = result.FirstError;
            else
                TempData["Success"] = successMessage;

            return id.HasValue
                ? RedirectToAction(actionName, new { id })
                : RedirectToAction(actionName);
        }

        private (string FirstName, string LastName) SplitFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return ("", "");

            var parts = fullName.Trim().Split(' ');
            var first = parts[0];
            var last = parts.Length > 1 ? string.Join(' ', parts.Skip(1)) : "";
            return (first, last);
        }

        private async Task<IActionResult> ExecuteUserOperation(Guid id, Func<Guid, Guid, CancellationToken, Task<Result>> operation, string successMessage, CancellationToken ct)
        {
            if (id == Guid.Empty) return BadRequest();
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized();

            var result = await operation(id, userId, ct);
            return RedirectWithMessage(result, successMessage, nameof(Details), id);
        }

        #endregion

        // ============================================================
        // LIST
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Index(UserFilterViewModel filter, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(new UserListViewModel { Filter = filter });

            var result = await _userService.GetPagedForAdminAsync(filter.PageNumber, filter.PageSize, ct);

            if (result.IsFailure)
            {
                TempData["Error"] = result.FirstError;
                return View(new UserListViewModel { Filter = filter });
            }

            var items = result.Value.Value
                .Select(UserMappings.ToListItemViewModel)
                .ToList();

            var viewModel = new UserListViewModel
            {
                Items = items,
                Filter = filter,
                TotalCount = result.Value.TotalCount
            };

            return View(viewModel);
        }

        // ============================================================
        // DETAILS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty) return BadRequest();

            var result = await _userService.GetByIdForAdminAsync(id, ct);

            if (result.IsFailure)
            {
                TempData["Error"] = result.FirstError;
                return RedirectToAction(nameof(Index));
            }

            var viewModel = UserMappings.ToDetailsViewModel(result.Value);
            return View(viewModel);
        }

        // ============================================================
        // CREATE
        // ============================================================
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(dto);

            var result = await _userService.CreateAsync(dto, ct);
            return RedirectWithMessage(result, "User created successfully.", nameof(Index));
        }

        // ============================================================
        // EDIT
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty) return BadRequest();

            var result = await _userService.GetByIdForAdminAsync(id, ct);
            if (result.IsFailure)
            {
                TempData["Error"] = result.FirstError;
                return RedirectToAction(nameof(Index));
            }

            var (firstName, lastName) = SplitFullName(result.Value.FullName);

            var viewModel = new UserUpdateDto
            {
                Id = result.Value.Id,
                FirstName = firstName,
                LastName = lastName,
                Email = result.Value.Email,
                Role = result.Value.Role
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserUpdateDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(dto);

            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var result = await _userService.UpdateAsync(dto, userId, ct);

            return RedirectWithMessage(
                result,
                "User updated successfully.",
                nameof(Index));
        }

        // ============================================================
        // DELETE / RESTORE
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Delete(Guid id, CancellationToken ct)
            => ExecuteUserOperation(id, _userService.DeleteAsync, "User deleted successfully.", ct);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Restore(Guid id, CancellationToken ct)
            => ExecuteUserOperation(id, _userService.RestoreAsync, "User restored successfully.", ct);

        // ============================================================
        // ROLE MANAGEMENT
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteToAdmin(Guid id, CancellationToken ct)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var result = await _userService.ChangeRoleAsync(
                id,
                UserRole.Admin,
                userId,
                ct);

            return RedirectWithMessage(
                result,
                "User promoted to Admin.",
                nameof(Details),
                id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DemoteToCustomer(Guid id, CancellationToken ct)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized();

            var result = await _userService.ChangeRoleAsync(
                id,
                UserRole.Customer,
                userId,
                ct);

            return RedirectWithMessage(
                result,
                "User demoted to Customer.",
                nameof(Details),
                id);
        }

        // ============================================================
        // CHANGE PASSWORD
        // ============================================================
        [HttpGet]
        public IActionResult ChangePassword(Guid id)
        {
            if (id == Guid.Empty) return BadRequest();
            return View(new UserChangePasswordViewModel { Id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UserChangePasswordViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized();

            var dto = new UserResetPasswordDto
            {
                Id = vm.Id,
                NewPassword = vm.NewPassword,
                ConfirmPassword = vm.ConfirmPassword
            };

            var result = await _accountService.AdminResetPasswordAsync(
                dto,
                userId,
                ct); return RedirectWithMessage(result, "Password changed successfully.", nameof(Details), vm.Id);
        }
    }
}