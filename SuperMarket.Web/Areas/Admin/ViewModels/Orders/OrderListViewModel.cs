namespace SuperMarket.Web.Areas.Admin.ViewModels.Orders;

public sealed class OrderListViewModel
{
    private IReadOnlyList<OrderListItemViewModel> _items
        = Array.Empty<OrderListItemViewModel>();

    public IReadOnlyList<OrderListItemViewModel> Items
    {
        get => _items;
        init => _items = value ?? Array.Empty<OrderListItemViewModel>();
    }

    public OrderFilterViewModel Filter { get; init; }
        = new();

    public int TotalCount { get; init; }

    public int TotalPages =>
        Filter.PageSize == 0
            ? 0
            : (int)Math.Ceiling(TotalCount / (double)Filter.PageSize);

    public bool HasPreviousPage =>
        Filter.PageNumber > 1;

    public bool HasNextPage =>
        Filter.PageNumber < TotalPages;
}