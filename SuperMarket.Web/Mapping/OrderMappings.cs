
using SuperMarket.Application.Common;
using SuperMarket.Application.DTOs.Orders;
using SuperMarket.Web.Areas.Admin.ViewModels.Orders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperMarket.Web.Areas.Admin.Mappings
{
    public static class OrderMappings
    {
        public static OrderListItemViewModel ToListItemViewModel(this OrderAdminDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new OrderListItemViewModel
            {
                Id = dto.Id,
                UserId = dto.UserId,
                UserName = dto.UserName ?? string.Empty,
                OrderStatus = MapOrderStatus(dto.OrderStatus),
                PaymentStatus = MapPaymentStatus(dto.PaymentStatus),
                TotalPrice = dto.TotalPrice,
                ItemsCount = dto.Items?.Count ?? 0,
                CreatedAt = dto.CreatedAt
            };
        }

        public static OrderDetailsViewModel ToDetailsViewModel(this OrderAdminDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            IReadOnlyList<OrderItemViewModel> items;

            if (dto.Items == null || dto.Items.Count == 0)
            {
                items = Array.Empty<OrderItemViewModel>();
            }
            else
            {
                items = dto.Items
                    .Select(x => x.ToItemViewModel())
                    .ToList()
                    .AsReadOnly(); // ReadOnlyCollection<T> پیاده‌سازی IReadOnlyList<T> است
            }

            return new OrderDetailsViewModel
            {
                Id = dto.Id,
                UserId = dto.UserId,
                UserName = dto.UserName ?? string.Empty,
                OrderStatus = MapOrderStatus(dto.OrderStatus),
                PaymentStatus = MapPaymentStatus(dto.PaymentStatus),
                TotalPrice = dto.TotalPrice,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                Items = items
            };
        }

        public static OrderItemViewModel ToItemViewModel(this OrderItemDetailDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            return new OrderItemViewModel
            {
                ProductId = dto.ProductId,
                ProductTitle = dto.ProductTitle ?? string.Empty,
                Price = dto.Price,
                Quantity = dto.Quantity
            };
        }

        private static OrderStatusViewModel MapOrderStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return OrderStatusViewModel.Pending;
            return status.Trim().ToLowerInvariant() switch
            {
                "pending" => OrderStatusViewModel.Pending,
                "paid" => OrderStatusViewModel.Paid,
                "shipped" => OrderStatusViewModel.Shipped,
                "delivered" => OrderStatusViewModel.Delivered,
                "cancelled" => OrderStatusViewModel.Cancelled,
                "refunded" => OrderStatusViewModel.Refunded,
                _ => OrderStatusViewModel.Pending
            };
        }

        private static PaymentStatusViewModel MapPaymentStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return PaymentStatusViewModel.Pending;
            return status.Trim().ToLowerInvariant() switch
            {
                "pending" => PaymentStatusViewModel.Pending,
                "paid" => PaymentStatusViewModel.Paid,
                "failed" => PaymentStatusViewModel.Failed,
                "refunded" => PaymentStatusViewModel.Refunded,
                _ => PaymentStatusViewModel.Pending
            };
        }
    }
}