using SuperMarket.Domain.Enums;

namespace SuperMarket.Web.Areas.Admin.ViewModels.Dashboard
{
    public class DashboardRecentOrderViewModel
    {
        public Guid Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public string UserFullName { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string TotalAmountFormatted => TotalAmount.ToString("N0");

        public string CreatedAtFormatted => CreatedAt.ToString("yyyy/MM/dd HH:mm");
    }
}