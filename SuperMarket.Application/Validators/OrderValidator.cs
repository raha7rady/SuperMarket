using FluentValidation;
using SuperMarket.Application.DTOs.Orders;
using System;

namespace SuperMarket.Application.Validators
{
    // Validator برای هر آیتم سفارش
    public sealed class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
    {
        private const int MaxQuantityPerItem = 100;

        public OrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
                .LessThanOrEqualTo(MaxQuantityPerItem)
                .WithMessage($"Quantity cannot exceed {MaxQuantityPerItem}.");
        }
    }

    // Validator برای ایجاد سفارش
    public sealed class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        private const int MaxItemsPerOrder = 100;

        public CreateOrderValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item.")
                .Must(items => items.Count <= MaxItemsPerOrder)
                .WithMessage($"Order cannot contain more than {MaxItemsPerOrder} items.");

            RuleForEach(x => x.Items)
                .SetValidator(new OrderItemDtoValidator());
        }
    }

    // Validator برای جزئیات آیتم‌های سفارش (Admin/Customer)
    public sealed class OrderItemDetailValidator : AbstractValidator<OrderItemDetailDto>
    {
        private const int MaxQuantityPerItem = 100;

        public OrderItemDetailValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId must be greater than 0.");

            RuleFor(x => x.ProductTitle)
                .NotEmpty().WithMessage("Product title is required.")
                .MaximumLength(200).WithMessage("Product title cannot exceed 200 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
                .LessThanOrEqualTo(MaxQuantityPerItem)
                .WithMessage($"Quantity cannot exceed {MaxQuantityPerItem}.");
        }
    }

    // Validator برای OrderAdminDto
    public sealed class OrderAdminValidator : AbstractValidator<OrderAdminDto>
    {
        public OrderAdminValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Order Id is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.OrderStatus)
                .IsInEnum().WithMessage("Invalid order status.");

            RuleFor(x => x.PaymentStatus)
                .IsInEnum().WithMessage("Invalid payment status.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must have at least one item.");

            RuleForEach(x => x.Items)
                .SetValidator(new OrderItemDetailValidator());
        }
    }

    // Validator برای OrderCustomerDto
    public sealed class OrderCustomerValidator : AbstractValidator<OrderCustomerDto>
    {
        public OrderCustomerValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Order Id is required.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item.");

            RuleForEach(x => x.Items)
                .SetValidator(new OrderItemDetailValidator());
        }
    }
}
