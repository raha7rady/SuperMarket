namespace SuperMarket.Application.DTOs.Categories
{
    public class CategoryCreateDto
    {
        public string Title { get; set; } = null!;
        public int DisplayOrder { get; set; } = 0;
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? Badge { get; set; }
    }
}
