
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Dashboard;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Domain.Enums;
using SuperMarket.Domain.Interfaces.Repositories;

namespace SuperMarket.Application.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    private const int RecentCount = 5;
    private const int LowStockThreshold = 5;

    public DashboardService(
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<DashboardDto>> GetDashboardDataAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;

        #region Users

        var totalUsers = await _userRepository.CountAsync(
            u => !u.IsDeleted,
            cancellationToken);

        var recentUsers = await _userRepository.ListPagedAsync(
            u => !u.IsDeleted,
            u => u.CreatedDate,
            false, // descending
            0,
            RecentCount,
            cancellationToken);

        #endregion

        #region Orders

        var allOrders = await _orderRepository.ListActiveAsync(
            0,
            RecentCount,
            cancellationToken);

        var totalOrders = allOrders.Count;

        var pendingOrders = allOrders.Count(
            o => o.OrderStatus == OrderStatus.Pending);

        var todayOrders = allOrders.Count(
            o => o.CreatedDate.UtcDateTime.Date == today);

        var paidOrders = allOrders
            .Where(o => o.PaymentStatus == PaymentStatus.Paid)
            .ToList();

        var todaySales = paidOrders.Sum(
            o => o.TotalPrice.Amount);

        var monthlySales = paidOrders.Sum(
            o => o.TotalPrice.Amount);

        var yearlySales = paidOrders.Sum(
            o => o.TotalPrice.Amount);

        var averageOrderValue = paidOrders.Count == 0
            ? 0
            : paidOrders.Average(o => o.TotalPrice.Amount);

        #endregion

        #region Products

        var totalProducts = await _productRepository.CountAsync(
            p => p.IsActive,
            cancellationToken);

        var lowStockProducts = await _productRepository.ListAsync(
            p => p.IsActive && p.Stock.Value <= LowStockThreshold,
            q => q.OrderBy(p => p.Stock.Value),
            0,
            RecentCount,
            cancellationToken);

        #endregion

        var dto = new DashboardDto
        {
            Statistics = new DashboardStatisticsDto
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                TodayOrders = todayOrders,
                LowStockProducts = lowStockProducts.Count
            },

            Sales = new DashboardSalesDto
            {
                TodaySales = todaySales,
                MonthlySales = monthlySales,
                YearlySales = yearlySales,
                AverageOrderValue = averageOrderValue
            },

            RecentOrders = new List<RecentOrderDto>(),

            RecentUsers = recentUsers
                .Select(u => new RecentUserDto
                {
                    Id = u.Id,
                    FullName = u.Name != null
                        ? $"{u.Name.FirstName} {u.Name.LastName}"
                        : string.Empty,

                    Email = u.Email.Value,

                    CreatedAt = u.CreatedDate.UtcDateTime
                })
                .ToList(),

            LowStockProducts = lowStockProducts
                .Select(p => new LowStockProductDto
                {
                    Id = p.Id,
                    Title = p.Title.Value,
                    StockQuantity = p.Stock.Value
                })
                .ToList()
        };

        return Result<DashboardDto>.Success(dto);
    }
}
