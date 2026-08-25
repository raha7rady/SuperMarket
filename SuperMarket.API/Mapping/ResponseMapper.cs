using SuperMarket.API.Contracts;
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Cart;
using SuperMarket.Application.DTOs.Orders;
using SuperMarket.Application.DTOs.Reviews;
using SuperMarket.Application.DTOs.Wishlist;

namespace SuperMarket.API.Mapping;

public static class ResponseMapper
{
    public static WishlistItemResponse ToResponse(this WishlistItemDto dto)
    {
        return new WishlistItemResponse
        {
            Id = dto.Id,
            ProductId = dto.ProductId,
            ProductTitle = dto.ProductTitle,
            ProductImageUrl = dto.ProductImageUrl,
            Price = dto.Price,
            IsInStock = dto.IsInStock,
            CreatedAt = dto.CreatedAt
        };
    }

    public static ReviewResponse ToResponse(this ReviewDto dto)
    {
        return new ReviewResponse
        {
            Id = dto.Id,
            ProductId = dto.ProductId,
            UserId = dto.UserId,
            ReviewerName = dto.ReviewerName,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = dto.CreatedAt
        };
    }

    public static CartResponse ToResponse(this CartCustomerDto dto)
    {
        return new CartResponse
        {
            Id = dto.Id,
            TotalItems = dto.TotalItems,
            TotalAmount = dto.TotalAmount,
            Items = dto.Items.Select(i => new CartItemResponse
            {
                Product = new ProductSummaryResponse
                {
                    Id = i.ProductId,
                    Title = i.ProductTitle,
                    ImageUrl = i.ProductImageUrl,
                    Price = i.Price
                },
                Quantity = i.Quantity,
                SubTotal = i.SubTotal
            }).ToList()
        };
    }

    public static OrderResponse ToResponse(this OrderCustomerDto dto)
    {
        return new OrderResponse
        {
            Id = dto.Id,
            OrderNumber = dto.OrderNumber,
            OrderStatus = dto.OrderStatus,
            PaymentStatus = dto.PaymentStatus,
            TotalPrice = dto.TotalPrice,
            CreatedAt = dto.CreatedAt,
            DeliveryOption = dto.DeliveryOption,
            PaymentMethod = dto.PaymentMethod,
            ShippingCost = dto.ShippingCost,
            CouponCode = dto.CouponCode,
            CouponDiscount = dto.CouponDiscount,
            FinalPayable = dto.FinalPayable,
            Recipient = dto.Recipient is null ? null : new OrderRecipientResponse
            {
                FullName = dto.Recipient.FullName,
                Phone = dto.Recipient.Phone,
                Province = dto.Recipient.Province,
                City = dto.Recipient.City,
                AddressLine = dto.Recipient.AddressLine,
                PostalCode = dto.Recipient.PostalCode,
                Plaque = dto.Recipient.Plaque,
                Unit = dto.Recipient.Unit,
                DeliveryNote = dto.Recipient.DeliveryNote
            },
            Items = dto.Items.Select(i => new OrderItemResponse
            {
                Product = new ProductSummaryResponse
                {
                    Id = i.ProductId,
                    Title = i.ProductTitle,
                    ImageUrl = null,
                    Price = i.Price
                },
                Quantity = i.Quantity,
                SubTotal = i.SubTotal
            }).ToList()
        };
    }

    public static PagedResponse<T> ToPagedResponse<T>(this PagedResult<T> result)
    {
        return new PagedResponse<T>
        {
            Items = result.Value,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            HasPreviousPage = result.HasPreviousPage,
            HasNextPage = result.HasNextPage
        };
    }
}
