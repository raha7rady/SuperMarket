namespace SuperMarket.Application.DTOs.Categories
{
    public class CategoryAdminDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? Badge { get; set; }
        public int ProductCount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
