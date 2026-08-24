using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket.Domain.Enums
{
    public enum ProductSortBy
    {
        DisplayOrder = 0,

        Newest = 1,

        NameAscending = 2,

        PriceLowToHigh = 3,

        PriceHighToLow = 4
    }
}
