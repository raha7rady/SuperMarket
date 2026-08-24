namespace SuperMarket.Web.Areas.Admin.ViewModels.Dashboard
{
    public class DashboardStatisticsViewModel
    {
        public int TotalUsers { get; set; }

        public int TotalProducts { get; set; }

        public int TotalOrders { get; set; }

        public int PendingOrders { get; set; }

        public int TodayOrders { get; set; }

        public int LowStockProducts { get; set; }
    }
}