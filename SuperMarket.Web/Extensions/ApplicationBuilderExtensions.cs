using SuperMarket.Web.Middlewares;

namespace SuperMarket.Web.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseWebMiddlewarePipeline(
        this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // امنیت
        app.UseMiddleware<SecurityHeadersMiddleware>();

        // لاگ درخواست
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseHttpsRedirection();

        app.UseResponseCompression();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();

        // Identity
        app.UseAuthentication();
        app.UseAuthorization();

        // مدیریت StatusCode
        app.UseStatusCodePagesWithReExecute("/Error/{0}");

        // Areas
        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

        // Default
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapControllers();

        return app;
    }
}