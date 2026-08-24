namespace SuperMarket.Web.Areas.Admin.ViewModels.Users
{
    public sealed class UserListViewModel
    {
        private IReadOnlyList<UserListItemViewModel> _items = Array.Empty<UserListItemViewModel>();

        public IReadOnlyList<UserListItemViewModel> Items
        {
            get => _items;
            init => _items = value ?? Array.Empty<UserListItemViewModel>();
        }

        public UserFilterViewModel Filter { get; init; } = new();

        public int TotalCount { get; init; }

        public int TotalPages =>
            Filter.PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)Filter.PageSize);

        public bool HasPreviousPage => Filter.PageNumber > 1;

        public bool HasNextPage => Filter.PageNumber < TotalPages;
    }
}