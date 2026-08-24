namespace SuperMarket.Web.Areas.Admin.ViewModels.Categories;

public sealed class CategoryListItemViewModel
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public int DisplayOrder { get; init; }

    public bool IsActive { get; init; }

    public int ProductCount { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }

    public string StatusLabel =>
        IsActive ? "Active" : "Inactive";
}