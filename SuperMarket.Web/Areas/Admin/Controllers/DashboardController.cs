using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Web.Areas.Admin.ViewModels.Dashboard;

namespace SuperMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SuperAdmin")]
public sealed class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IMapper _mapper;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService,
                               IMapper mapper,
                               ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetDashboardDataAsync(cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogError(
                "Dashboard loading failed. ErrorCode: {ErrorCode}, Errors: {Errors}",
                result.ErrorCode,
                string.Join(" | ", result.Errors));

            TempData["ErrorMessage"] = result.FirstError ?? "Unable to load dashboard data at this time.";
            return View(new DashboardViewModel());
        }

        var viewModel = _mapper.Map<DashboardViewModel>(result.Value);
        return View(viewModel);
    }
}