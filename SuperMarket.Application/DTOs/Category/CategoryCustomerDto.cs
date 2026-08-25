using System;

namespace SuperMarket.Application.DTOs.Categories
{
    public class CategoryCustomerDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? Badge { get; set; }
        public int ItemCount { get; set; }
    }
}
