

namespace SuperMarket.Application.DTOs.Categories
{
    public class CategoryCreateDto
    {
        public string Title { get; set; } = null!;
        public int DisplayOrder { get; set; } = 0;
    }
}
