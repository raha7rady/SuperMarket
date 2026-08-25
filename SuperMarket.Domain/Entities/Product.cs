
using SuperMarket.Domain.Common;
using SuperMarket.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace SuperMarket.Domain.Entities;

public class Product : AuditableEntity
{
    public ProductTitle Title { get; private set; } = null!;

    public Toman Price { get; private set; }

    public StockQuantity Stock { get; private set; }

    public string ImageUrl { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public string Slug { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public DisplayOrder SortOrder { get; private set; } = null!;
    public Guid CategoryId { get; private set; }

    public Category Category { get; private set; } = null!;

    public string? MetaTitle { get; private set; }

    public string? MetaDescription { get; private set; }

    public Toman? CompareAtPrice { get; private set; }

    public string? Brand { get; private set; }

    public string? Barcode { get; private set; }

    public string? Unit { get; private set; }

    public string? Tags { get; private set; }

    public string? DietaryTags { get; private set; }

    public string? GalleryImages { get; private set; }

    public bool IsSpecialDeal { get; private set; }

    public bool IsBestSeller { get; private set; }

    public DateTimeOffset? DealEndTime { get; private set; }

    public bool HasValidDiscount =>
        CompareAtPrice is not null && CompareAtPrice.Amount > Price.Amount;

    public int? DiscountPercent =>
        HasValidDiscount
            ? (int)Math.Round((1 - Price.Amount / CompareAtPrice!.Amount) * 100)
            : null;

    private Product()
    {
    }

    public Product(
        string title,
        decimal price,
        int stock,
        string imageUrl,
        Guid categoryId,
        string description,
        string slug,
        int displayOrder = 0)
    {
        SetTitle(title);
        SetPrice(price);
        SetStock(stock);
        SetImageUrl(imageUrl);
        SetCategory(categoryId);
        SetDescription(description);
        SetSlug(slug);
        SetDisplayOrder(displayOrder);

        IsActive = true;
    }

    public void Update(
        string title,
        decimal price,
        int stock,
        string imageUrl,
        Guid categoryId,
        string description,
        string slug,
        int displayOrder,
        Guid? modifiedBy = null)
    {
        EnsureNotDeleted();

        SetTitle(title);

        SetPrice(price);

        SetStock(stock);

        SetImageUrl(imageUrl);

        SetCategory(categoryId);

        SetDescription(description);

        SetSlug(slug);

        SetDisplayOrder(displayOrder);

        if (modifiedBy.HasValue)
        {
            SetModified(modifiedBy.Value);
        }
    }

    public void UpdateCatalogDetails(
        decimal? compareAtPrice,
        string? brand,
        string? barcode,
        string? unit,
        IEnumerable<string>? tags,
        IEnumerable<string>? dietaryTags,
        IEnumerable<string>? galleryImages,
        bool isSpecialDeal,
        bool isBestSeller,
        DateTimeOffset? dealEndTime,
        Guid? modifiedBy = null)
    {
        EnsureNotDeleted();

        SetCompareAtPrice(compareAtPrice);
        Brand = NormalizeOptional(brand, MaxBrandLength);
        Barcode = NormalizeOptional(barcode, MaxBarcodeLength);
        Unit = NormalizeOptional(unit, MaxUnitLength);
        Tags = JoinList(tags);
        DietaryTags = JoinList(dietaryTags);
        GalleryImages = JoinList(galleryImages);
        IsSpecialDeal = isSpecialDeal;
        IsBestSeller = isBestSeller;
        DealEndTime = dealEndTime;

        if (modifiedBy.HasValue)
        {
            SetModified(modifiedBy.Value);
        }
    }

    public void IncreaseStock(
        int quantity,
        Guid? modifiedBy = null)
    {
        EnsureNotDeleted();

        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));
        }

        Stock = StockQuantity.Create(
            Stock.Value + quantity);

        if (modifiedBy.HasValue)
        {
            SetModified(modifiedBy.Value);
        }
    }

    public void DecreaseStock(
        int quantity,
        Guid? modifiedBy = null)
    {
        EnsureNotDeleted();

        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));
        }

        if (quantity > Stock.Value)
        {
            throw new InvalidOperationException(
                "Insufficient product stock.");
        }

        Stock = StockQuantity.Create(
            Stock.Value - quantity);

        if (modifiedBy.HasValue)
        {
            SetModified(modifiedBy.Value);
        }
    }

    public void SetActive(
        bool isActive,
        Guid? modifiedBy = null)
    {
        EnsureNotDeleted();

        if (IsActive == isActive)
        {
            return;
        }

        IsActive = isActive;

        if (modifiedBy.HasValue)
        {
            SetModified(modifiedBy.Value);
        }
    }

    public override void SoftDelete(Guid deletedBy)
    {
        if (IsDeleted)
        {
            return;
        }

        base.SoftDelete(deletedBy);
    }

    private void SetTitle(string title)
    {
        Title = ProductTitle.Create(title);
    }

    private void SetPrice(decimal price)
    {
        Price = Toman.Create(price);
    }

    private void SetStock(int stock)
    {
        Stock = StockQuantity.Create(stock);
    }

    private const int MaxImageUrlLength = 500;

    private void SetImageUrl(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new ArgumentException(
                "Image url is required.",
                nameof(imageUrl));
        }

        imageUrl = imageUrl.Trim();

        if (imageUrl.Length > MaxImageUrlLength)
        {
            throw new ArgumentException(
                $"Image url cannot exceed {MaxImageUrlLength} characters.",
                nameof(imageUrl));
        }

        ImageUrl = imageUrl;
    }

    private void SetCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "Category id is invalid.",
                nameof(categoryId));
        }

        CategoryId = categoryId;
    }

    private const int MaxDescriptionLength = 4000;

    private void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Description is required.",
                nameof(description));
        }

        description = description.Trim();

        if (description.Length > MaxDescriptionLength)
        {
            throw new ArgumentException(
                $"Description cannot exceed {MaxDescriptionLength} characters.",
                nameof(description));
        }

        Description = description;
    }

    private void SetSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException(
                "Slug is required.",
                nameof(slug));
        }

        slug = slug.Trim().ToLowerInvariant();

        slug = Regex.Replace(slug, @"\s+", "-");

        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

        slug = Regex.Replace(slug, @"-+", "-");

        slug = slug.Trim('-');

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException(
                "Slug is invalid.",
                nameof(slug));
        }

        Slug = slug;
    }

    private void SetDisplayOrder(int displayOrder)
    {
        SortOrder = DisplayOrder.Create(displayOrder);
    }

    private const int MaxBrandLength = 100;
    private const int MaxBarcodeLength = 50;
    private const int MaxUnitLength = 20;
    private const int MaxTagLength = 40;
    private const int MaxTagsCount = 20;
    private const string ListDelimiter = ";";

    private void SetCompareAtPrice(decimal? compareAtPrice)
    {
        CompareAtPrice = compareAtPrice.HasValue && compareAtPrice.Value > 0
            ? Toman.Create(compareAtPrice.Value)
            : null;
    }

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        value = value.Trim();

        if (value.Length > maxLength)
        {
            throw new ArgumentException(
                $"Value cannot exceed {maxLength} characters.",
                nameof(value));
        }

        return value;
    }

    private static string? JoinList(IEnumerable<string>? values)
    {
        if (values is null)
        {
            return null;
        }

        var cleaned = values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v.Trim())
            .Where(v => v.Length <= MaxTagLength)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(MaxTagsCount)
            .ToList();

        return cleaned.Count == 0 ? null : string.Join(ListDelimiter, cleaned);
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException(
                "Cannot modify deleted product.");
        }
    }
}