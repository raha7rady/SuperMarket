

namespace SuperMarket.Application.DTOs.Categories
{
    public class CategoryUpdateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
