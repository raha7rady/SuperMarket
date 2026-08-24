
using System;
using System.Collections.Generic;

namespace SuperMarket.Application.DTOs.Cart
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
