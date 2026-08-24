using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket.Application.DTOs.Categories
{
    public sealed class CategoryLookupDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
