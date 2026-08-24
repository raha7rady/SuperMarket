
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using SuperMarket.Web.Authorization;

namespace SuperMarket.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebLayer(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var mvcBuilder = services.AddControllersWithViews(options =>
        {
            options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
        });

        if (environment.IsDevelopment())
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }

        services.AddAutoMapper(typeof(Program).Assembly);

        services.AddMemoryCache();

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        services.AddProblemDetails();

        //services.AddWebAuthorization();

        return services;
    }
}