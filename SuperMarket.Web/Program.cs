using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SuperMarket.Application;
using SuperMarket.Infrastructure;
using SuperMarket.Infrastructure.Identity;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// 1️⃣ Services Registration
// =====================================================

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWebLayer(builder.Configuration, builder.Environment)
    .AddCustomAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";

    options.AccessDeniedPath =
        "/Account/AccessDenied";

    options.Cookie.Name =
        "SuperMarket.Auth";

    options.Cookie.HttpOnly = true;

    options.Cookie.SameSite =
        SameSiteMode.Lax;

    // هماهنگ با سیاست Cookie.SecurePolicy مربوط به Session
    // (ServiceCollectionExtensions.AddWebLayer) — هر دو Cookie حساس
    // برنامه باید یک سیاست امنیتی یکسان داشته باشند.
    options.Cookie.SecurePolicy =
        CookieSecurePolicy.Always;

    options.ExpireTimeSpan =
        TimeSpan.FromHours(2);

    options.SlidingExpiration = true;
});

// =====================================================
// 2️⃣ Swagger
// =====================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "SuperMarket API",
            Version = "v1"
        });
});

var app = builder.Build();

// =====================================================
// 3️⃣ Database & Identity Seed
// =====================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var dbContext =
            services.GetRequiredService<SuperMarketDbContext>();

        var strategy =
            dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await IdentitySeeder.SeedAsync(
                services,
                builder.Configuration);
        });
    }
    catch (Exception ex)
    {
        var logger =
            services.GetRequiredService<ILogger<Program>>();

        logger.LogError(
            ex,
            "Error occurred while seeding Identity data.");
    }
}

// =====================================================
// 4️⃣ Dev-only tooling (Developer Exception Page + Swagger UI)
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "SuperMarket API v1");
    });
}

// =====================================================
// 5️⃣ Middleware Pipeline
// =====================================================
// همه‌ی موارد pipeline (Exception/HSTS در Production، Security Headers،
// Request Logging، HTTPS Redirection، Response Compression، Static Files،
// Routing، Session، Authentication، Authorization، Status Code Pages و
// Route‌های Area/Default) به‌صورت متمرکز در همین Extension مدیریت می‌شوند
// تا از دو نسخه‌ی متفاوت و ناهماهنگ pipeline جلوگیری شود.

app.UseWebMiddlewarePipeline();

// =====================================================
// 6️⃣ Run Application
// =====================================================

app.Run();