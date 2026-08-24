namespace SuperMarket.Web.Areas.Admin.ViewModels.Products;

public sealed class ProductFilterViewModel
{
    public string? SearchTerm { get; set; }

    public Guid? CategoryId { get; set; }

    public bool? IsActive { get; set; }

    public string? SortColumn { get; set; }

    public string? SortDirection { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}