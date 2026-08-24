

using System;

namespace SuperMarket.Application.DTOs.Categories
{

    public class CategoryCustomerDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}
