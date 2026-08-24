using Microsoft.AspNetCore.Mvc;
using SuperMarket.Web.ViewComponents.Models;

namespace SuperMarket.Web.ViewComponents;

public sealed class HeroBannerViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var model = new HeroBannerViewModel();

        return View(model);
    }
}