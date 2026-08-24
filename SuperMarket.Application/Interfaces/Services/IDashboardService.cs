using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Dashboard;

namespace SuperMarket.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<Result<DashboardDto>> GetDashboardDataAsync(
            CancellationToken cancellationToken = default);
    }
}