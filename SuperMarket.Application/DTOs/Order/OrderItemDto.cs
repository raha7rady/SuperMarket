

using System;
using System.Collections.Generic;

namespace SuperMarket.Application.DTOs.Orders
{

    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
