using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SuperMarket.Application.DTOs.Cart;
using SuperMarket.Application.DTOs.Orders;
using SuperMarket.Application.DTOs.Products;
using SuperMarket.Application.DTOs.Users;
using SuperMarket.Application.Interfaces.Services;
using SuperMarket.Application.Services;
using SuperMarket.Application.Validators;
using System;

namespace SuperMarket.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IWishlistService, WishlistService>();

        services.AddScoped<IValidator<ProductCreateDto>, ProductValidator>();
        services.AddScoped<IValidator<ProductUpdateDto>, ProductUpdateValidator>();
        services.AddScoped<IValidator<CreateOrderDto>, CreateOrderValidator>();
        services.AddScoped<IValidator<CartCreateDto>, CartCreateValidator>();
        services.AddScoped<IValidator<CartItemDto>, CartItemDtoValidator>();
        services.AddScoped<IValidator<CartUpdateItemDto>, CartUpdateItemValidator>();
        services.AddScoped<IValidator<UserCreateDto>, UserValidator>();
        services.AddScoped<IValidator<UserUpdateDto>, UserUpdateValidator>();

        return services;
    }
}
