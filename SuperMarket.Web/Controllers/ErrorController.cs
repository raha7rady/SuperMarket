using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SuperMarket.Web.ViewModels;

namespace SuperMarket.Web.Controllers;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class ErrorController : Controller
{
    [HttpGet("/Error")]
    public IActionResult Index()
    {
        var exceptionFeature =
            HttpContext.Features
                .Get<IExceptionHandlerPathFeature>();

        var model = new ErrorViewModel
        {
            RequestId = HttpContext.TraceIdentifier,
            ErrorMessage =
                exceptionFeature?.Error.Message
                ?? "Unexpected error occurred.",
            Path = exceptionFeature?.Path
        };

        return View(model);
    }

    [HttpGet("/Error/404")]
    public IActionResult NotFoundPage()
    {
        Response.StatusCode = 404;

        return View("NotFound");
    }
}