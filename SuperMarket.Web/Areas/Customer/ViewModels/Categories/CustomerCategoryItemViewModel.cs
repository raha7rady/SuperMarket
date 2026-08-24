namespace SuperMarket.Web.Areas.Customer.ViewModels.Categories;

public sealed class CustomerCategoryItemViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsSelected { get; set; }
}