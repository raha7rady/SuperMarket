namespace SuperMarket.Web.Areas.Admin.ViewModels.Dashboard
{
    public class DashboardRecentUserViewModel
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }

        public string CreatedAtFormatted => CreatedAt.ToString("yyyy/MM/dd");
    }
}