namespace SuperMarket.Web.Areas.Admin.ViewModels.Users
{
    public sealed class UserFilterViewModel
    {
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 100;

        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value <= 0 ? 1 : value;
        }

        private int _pageSize = DefaultPageSize;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0) _pageSize = DefaultPageSize;
                else _pageSize = value > MaxPageSize ? MaxPageSize : value;
            }
        }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

        public bool? IsDeleted { get; set; }

        public string SortBy { get; set; } = "CreatedAt";

        public bool SortDescending { get; set; } = true;
    }
}