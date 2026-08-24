namespace SuperMarket.Web.Areas.Admin.ViewModels.Dashboard
{
    public class DashboardSalesViewModel
    {
        public decimal TodaySales { get; set; }

        public decimal MonthlySales { get; set; }

        public decimal YearlySales { get; set; }

        public decimal AverageOrderValue { get; set; }

        public string TodaySalesFormatted => TodaySales.ToString("N0");

        public string MonthlySalesFormatted => MonthlySales.ToString("N0");

        public string YearlySalesFormatted => YearlySales.ToString("N0");

        public string AverageOrderValueFormatted => AverageOrderValue.ToString("N0");
    }
}