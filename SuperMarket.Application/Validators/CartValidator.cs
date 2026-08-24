using FluentValidation;
using SuperMarket.Application.DTOs.Cart;
using System;

namespace SuperMarket.Application.Validators
{
    // =========================================
    // Cart Create Validator
    // =========================================
    public sealed class CartCreateValidator : AbstractValidator<CartCreateDto>
    {
        private const int MaxQuantityPerItem = 100;

        public CartCreateValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            // Items in CartCreateDto are of type CartItemDto (only ProductId and Quantity)
            //RuleForEach(x => x.Items)
            //    .SetValidator(new CartItemDtoValidator());
        }
    }

    // =========================================
    // Validator for CartItemDto (used in CartCreateDto)
    // =========================================
    public sealed class CartItemDtoValidator : AbstractValidator<CartItemDto>
    {
        private const int MaxQuantityPerItem = 100;

        public CartItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
                .LessThanOrEqualTo(MaxQuantityPerItem)
                .WithMessage($"Quantity cannot exceed {MaxQuantityPerItem}.");
        }
    }

    // =========================================
    // Cart Update Item Validator
    // =========================================
    public sealed class CartUpdateItemValidator : AbstractValidator<CartUpdateItemDto>
    {
        private const int MaxQuantityPerItem = 100;

        public CartUpdateItemValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
                .LessThanOrEqualTo(MaxQuantityPerItem)
                .WithMessage($"Quantity cannot exceed {MaxQuantityPerItem}.");
        }
    }

    // =========================================
    // Cart Item Detail Validator (used in Admin DTO)
    // =========================================
    public sealed class CartItemDetailValidator : AbstractValidator<CartItemDetailDto>
    {
        private const int MaxQuantityPerItem = 100;

        public CartItemDetailValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId is required.");

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

    // =========================================
    // Cart Admin Validator
    // =========================================
    public sealed class CartAdminValidator : AbstractValidator<CartAdminDto>
    {
        public CartAdminValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Cart Id is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            // Items in CartAdminDto are CartItemDetailDto
            RuleForEach(x => x.Items)
                .SetValidator(new CartItemDetailValidator());
        }
    }
}
