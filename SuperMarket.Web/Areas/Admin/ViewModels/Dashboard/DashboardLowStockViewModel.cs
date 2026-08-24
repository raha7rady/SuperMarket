namespace SuperMarket.Web.Areas.Admin.ViewModels.Dashboard
{
    public class DashboardLowStockViewModel
    {
        public Guid Id { get; set; }

        public string ProductTitle { get; set; } = string.Empty;

        public int StockQuantity { get; set; }

        public int Threshold { get; set; } = 5;

        public bool IsCritical => StockQuantity <= Threshold;
    }
}