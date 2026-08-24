
namespace SuperMarket.Application.DTOs.Products
{
    public class ProductAdminDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
