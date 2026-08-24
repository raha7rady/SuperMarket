namespace SuperMarket.Web.Areas.Admin.ViewModels.Orders;

public sealed class OrderFilterViewModel
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
            if (value <= 0)
                _pageSize = DefaultPageSize;
            else
                _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }

    public Guid? OrderId { get; set; }

    public string? UserName { get; set; }

    public OrderStatusViewModel? OrderStatus { get; set; }

    public PaymentStatusViewModel? PaymentStatus { get; set; }

    public DateTimeOffset? FromDate { get; set; }

    public DateTimeOffset? ToDate { get; set; }

    public string SortBy { get; set; } = "CreatedAt";

    public bool SortDescending { get; set; } = true;
}