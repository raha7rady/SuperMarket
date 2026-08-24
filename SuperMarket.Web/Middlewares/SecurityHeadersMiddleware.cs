
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SuperMarket.Web.Middlewares
{
    public sealed class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public SecurityHeadersMiddleware(
            RequestDelegate next,
            ILogger<SecurityHeadersMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                var headers = context.Response.Headers;

                // 1. Security Headers
                headers.TryAdd("X-Content-Type-Options", "nosniff");
                headers.TryAdd("X-Frame-Options", "DENY");
                headers.TryAdd("X-XSS-Protection", "1; mode=block");
                headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");
                headers.TryAdd("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

                // 2. HSTS (Production Only)
                if (!_environment.IsDevelopment())
                {
                    headers.TryAdd("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
                }

                // 3. Content Security Policy (CSP)
                // اجازه به اسکریپت‌ها و استایل‌های داخلی، و همچنین CDNهای رایج
                headers.TryAdd(
                    "Content-Security-Policy",
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                    "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://fonts.googleapis.com; " +
                    "img-src 'self' data: https:; " +
                    "font-src 'self' https://fonts.gstatic.com https://cdn.jsdelivr.net; " +
                    "connect-src 'self'; " +
                    "frame-ancestors 'none';");

                // 4. Cache Control (برای صفحات ادمین بهتر است cache نشوند)
                if (context.Request.Path.StartsWithSegments("/Admin"))
                {
                    headers.TryAdd("Cache-Control", "no-cache, no-store, must-revalidate");
                    headers.TryAdd("Pragma", "no-cache");
                    headers.TryAdd("Expires", "0");
                }

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
