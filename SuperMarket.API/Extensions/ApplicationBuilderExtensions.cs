namespace SuperMarket.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApiMiddlewarePipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler();
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors(ServiceCollectionExtensions.CorsPolicyName);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
