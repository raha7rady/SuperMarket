

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SuperMarket.Web.Middlewares
{
    public sealed class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                LogRequest(context, stopwatch.ElapsedMilliseconds);
            }
        }

        private void LogRequest(HttpContext context, long elapsedMilliseconds)
        {
            var statusCode = context.Response.StatusCode;
            var level = statusCode >= 500 ? LogLevel.Error
                      : statusCode >= 400 ? LogLevel.Warning
                      : LogLevel.Information;

            _logger.Log(level,
                "HTTP {Method} {Path} responded {StatusCode} in {Elapsed} ms | TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                elapsedMilliseconds,
                context.TraceIdentifier);
        }
    }
}
