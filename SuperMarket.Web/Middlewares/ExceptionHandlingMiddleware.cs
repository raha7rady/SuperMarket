
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SuperMarket.Application.Exceptions;

namespace SuperMarket.Web.Middlewares
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IProblemDetailsService? _problemDetailsService;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment environment,
            IProblemDetailsService? problemDetailsService = null)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
            _problemDetailsService = problemDetailsService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // اگر پاسخ شروع شده باشد، دیگر نمی‌توانیم چیزی بنویسیم
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning(ex, "Response has already started for {Path}. Cannot handle exception.", context.Request.Path);
                    throw;
                }

                _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId} | Path: {Path}", context.TraceIdentifier, context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.Clear();

            var problemDetails = new ProblemDetails();
            var statusCode = HttpStatusCode.InternalServerError;
            var isJsonRequest = IsJsonRequest(context);

            // تعیین وضعیت و پیام بر اساس نوع استثنا
            switch (exception)
            {
                case BusinessRuleException businessEx:
                    statusCode = HttpStatusCode.UnprocessableEntity; // 422
                    problemDetails.Title = "خطای قوانین کسب‌وکار";
                    problemDetails.Detail = string.Join(", ", businessEx.Errors);
                    problemDetails.Extensions.Add("errors", businessEx.Errors);
                    break;

                case NotFoundException notFoundEx:
                    statusCode = HttpStatusCode.NotFound; // 404
                    problemDetails.Title = "منبع یافت نشد";
                    problemDetails.Detail = notFoundEx.Message;
                    break;

                case UnauthorizedActionException unauthEx:
                    statusCode = HttpStatusCode.Forbidden; // 403
                    problemDetails.Title = "عدم دسترسی";
                    problemDetails.Detail = unauthEx.Message;
                    break;

                case ValidationException validationEx:
                    statusCode = HttpStatusCode.BadRequest; // 400
                    problemDetails.Title = "خطای اعتبارسنجی";
                    problemDetails.Detail = string.Join(", ", validationEx.Errors);
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError; // 500
                    problemDetails.Title = "خطای سرور";
                    problemDetails.Detail = "متأسفانه خطایی غیرمنتظره رخ داده است.";
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            problemDetails.Instance = context.Request.Path;
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;

            // افزودن جزئیات فنی فقط در محیط توسعه
            if (_environment.IsDevelopment())
            {
                problemDetails.Extensions.Add("stackTrace", exception.StackTrace);
                problemDetails.Extensions.Add("innerException", exception.InnerException?.ToString());
            }

            if (isJsonRequest)
            {
                context.Response.ContentType = "application/json";

                // استفاده از سرویس استاندارد اگر موجود باشد
                if (_problemDetailsService != null)
                {
                    await _problemDetailsService.WriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = problemDetails
                    });
                }
                else
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = false
                    };
                    var result = JsonSerializer.Serialize(problemDetails, jsonOptions);
                    await context.Response.WriteAsync(result);
                }
            }
            else
            {
                // برای درخواست‌های HTML (مثل پنل ادمین)
                // ذخیره خطا در Items برای نمایش در View خطا
                context.Items["Exception"] = exception;

                // تغییر کد وضعیت به 500 برای ریدایرکت به صفحه خطای HTML
                context.Response.StatusCode = 500;

                // ریدایرکت به صفحه خطای عمومی یا نمایش در Layout
                // نکته: اگر می‌خواهید پیام خاص را در Layout نشان دهید، می‌توانید در TempData یا ViewContext قرار دهید
                // اما ریدایرکت ساده‌ترین روش برای Views است
                context.Response.Redirect($"/Error?traceId={context.TraceIdentifier}");
            }
        }

        private static bool IsJsonRequest(HttpContext context)
        {
            // بررسی مسیرهای API
            if (context.Request.Path.StartsWithSegments("/api"))
                return true;

            // بررسی Accept Header (برای APIهایی که در مسیرهای معمولی هستند)
            var acceptHeader = context.Request.Headers["Accept"].ToString();
            return acceptHeader.Contains("application/json", StringComparison.OrdinalIgnoreCase);
        }
    }
}