namespace SuperMarket.Application.DTOs.Products
{
    public class ProductAdminDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string? Brand { get; set; }
        public string? Barcode { get; set; }
        public string? Unit { get; set; }
        public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> DietaryTags { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> GalleryImages { get; set; } = Array.Empty<string>();
        public bool IsSpecialDeal { get; set; }
        public bool IsBestSeller { get; set; }
        public DateTimeOffset? DealEndTime { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
