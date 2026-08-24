namespace SuperMarket.Web.Areas.Admin.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public DashboardStatisticsViewModel Statistics { get; set; } = new();

        public DashboardSalesViewModel Sales { get; set; } = new();

        public List<DashboardRecentOrderViewModel> RecentOrders { get; set; } = new();

        public List<DashboardRecentUserViewModel> RecentUsers { get; set; } = new();

        public List<DashboardLowStockViewModel> LowStockProducts { get; set; } = new();
    }
}