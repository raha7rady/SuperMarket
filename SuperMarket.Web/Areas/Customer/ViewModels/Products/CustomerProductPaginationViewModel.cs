namespace SuperMarket.Web.Areas.Customer.ViewModels.Products;

public sealed class CustomerProductPaginationViewModel
{
    private const int MaxPagesToShow = 5;

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages =>
        PageSize <= 0
            ? 1
            : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public int PreviousPage => Math.Max(1, PageNumber - 1);

    public int NextPage => Math.Min(TotalPages, PageNumber + 1);

    public int StartPage
    {
        get
        {
            var start = PageNumber - 2;

            if (start < 1)
                start = 1;

            if (start + MaxPagesToShow - 1 > TotalPages)
                start = Math.Max(
                    1,
                    TotalPages - MaxPagesToShow + 1);

            return start;
        }
    }

    public int EndPage =>
        Math.Min(
            TotalPages,
            StartPage + MaxPagesToShow - 1);
}