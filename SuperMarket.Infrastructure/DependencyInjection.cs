

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Identity;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Infrastructure.Repositories;
using SuperMarket.Infrastructure.Repositories.Base;
using SuperMarket.Infrastructure.Services;
using System;
using SuperMarket.Application.Interfaces.Services;

namespace SuperMarket.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // -----------------------
        // 1️⃣ DbContext
        // -----------------------
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("DefaultConnection is not configured.");

        services.AddDbContext<SuperMarketDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                sql.MigrationsAssembly(typeof(SuperMarketDbContext).Assembly.FullName);
            }));

        // -----------------------
        // 2️⃣ Identity (بهینه و کامل)
        // -----------------------
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<SuperMarketDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

        // -----------------------
        // 3️⃣ Repositories
        // -----------------------
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

        // -----------------------
        // 4️⃣ Services
        // -----------------------
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();



        // -----------------------
        // 5️⃣ Password Hasher Service
        // -----------------------
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IAccountService, AccountService>();


        services.AddScoped<IIdentitySyncService, IdentitySyncService>();


        return services;
    }
}
