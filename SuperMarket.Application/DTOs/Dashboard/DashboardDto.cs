using SuperMarket.Domain.Enums;

namespace SuperMarket.Application.DTOs.Dashboard
{
    public sealed class DashboardDto
    {
        public DashboardStatisticsDto Statistics { get; init; } = new();

        public DashboardSalesDto Sales { get; init; } = new();

        public IReadOnlyList<RecentOrderDto> RecentOrders { get; init; }
            = new List<RecentOrderDto>();

        public IReadOnlyList<RecentUserDto> RecentUsers { get; init; }
            = new List<RecentUserDto>();

        public IReadOnlyList<LowStockProductDto> LowStockProducts { get; init; }
            = new List<LowStockProductDto>();
    }

    public sealed class DashboardStatisticsDto
    {
        public int TotalUsers { get; init; }
        public int TotalProducts { get; init; }
        public int TotalOrders { get; init; }
        public int PendingOrders { get; init; }
        public int TodayOrders { get; init; }
        public int LowStockProducts { get; init; }
    }

    public sealed class DashboardSalesDto
    {
        public decimal TodaySales { get; init; }
        public decimal MonthlySales { get; init; }
        public decimal YearlySales { get; init; }
        public decimal AverageOrderValue { get; init; }
    }

    public sealed class RecentOrderDto
    {
        public Guid Id { get; init; }

        public string OrderNumber { get; init; } = string.Empty;

        public string UserFullName { get; init; } = string.Empty;

        public decimal TotalAmount { get; init; }

        public OrderStatus OrderStatus { get; init; }

        public PaymentStatus PaymentStatus { get; init; }

        public DateTimeOffset CreatedAt { get; init; }
    }

    public sealed class RecentUserDto
    {
        public Guid Id { get; init; }

        public string FullName { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public DateTimeOffset CreatedAt { get; init; }
    }

    public sealed class LowStockProductDto
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = string.Empty;

        public int StockQuantity { get; init; }
    }
}