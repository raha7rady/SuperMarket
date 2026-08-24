using FluentValidation;
using SuperMarket.Application.DTOs.Products;
using SuperMarket.Application.Products.Queries;
using System;
using System.Linq.Expressions;

namespace SuperMarket.Application.Validators
{
    public abstract class ProductBaseValidator<T> : AbstractValidator<T>
        where T : class
    {
        protected const int MaxTitleLength = 200;
        protected const int MinTitleLength = 3;
        protected const int MaxDisplayOrder = 1000;
        protected const decimal MaxPrice = 1_000_000_000m;

        protected void ApplyCommonRules(
            Expression<Func<T, string>> titleExpression,
            Expression<Func<T, decimal>> priceExpression,
            Expression<Func<T, int>> stockExpression,
            Expression<Func<T, string>> imageUrlExpression,
            Expression<Func<T, Guid>> categoryIdExpression, // <-- اصلاح شد
            Expression<Func<T, int>> displayOrderExpression)
        {
            RuleFor(titleExpression)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Product title is required.")
                .MinimumLength(MinTitleLength).WithMessage($"Product title must be at least {MinTitleLength} characters.")
                .MaximumLength(MaxTitleLength).WithMessage($"Product title cannot exceed {MaxTitleLength} characters.")
                .Must(title => !string.IsNullOrWhiteSpace(title?.Trim()))
                .WithMessage("Product title cannot be whitespace only.");

            RuleFor(priceExpression)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("Price must be greater than zero.")
                .LessThanOrEqualTo(MaxPrice).WithMessage($"Price cannot exceed {MaxPrice}.")
                .PrecisionScale(18, 2, false)
                .WithMessage("Price must have maximum 2 decimal places.");

            RuleFor(stockExpression)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0).WithMessage("Stock must be zero or greater.")
                .LessThan(int.MaxValue).WithMessage("Stock value is too large.");

            RuleFor(imageUrlExpression)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ImageUrl is required.")
                .Must(uri =>
                {
                    if (string.IsNullOrWhiteSpace(uri)) return false;
                    return Uri.TryCreate(uri.Trim(), UriKind.RelativeOrAbsolute, out _);
                })
                .WithMessage("ImageUrl must be a valid URI.");

            RuleFor(categoryIdExpression)
                .NotEmpty().WithMessage("CategoryId is required."); // <-- اصلاح شد

            RuleFor(displayOrderExpression)
                .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder cannot be negative.")
                .LessThanOrEqualTo(MaxDisplayOrder)
                .WithMessage($"DisplayOrder cannot exceed {MaxDisplayOrder}.");
        }
    }

    public sealed class ProductValidator : ProductBaseValidator<ProductCreateDto>
    {
        public ProductValidator()
        {
            ApplyCommonRules(
                p => p.Title,
                p => p.Price,
                p => p.Stock,
                p => p.ImageUrl,
                p => p.CategoryId,
                p => p.DisplayOrder
            );
        }
    }

    public sealed class ProductUpdateValidator : ProductBaseValidator<ProductUpdateDto>
    {
        public ProductUpdateValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("Product Id is required for update.");

            ApplyCommonRules(
                p => p.Title,
                p => p.Price,
                p => p.Stock,
                p => p.ImageUrl,
                p => p.CategoryId,
                p => p.DisplayOrder
            );
        }
    }
}
