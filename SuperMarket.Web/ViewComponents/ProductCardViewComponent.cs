using Microsoft.AspNetCore.Mvc;
using SuperMarket.Web.Areas.Customer.ViewModels.Products;

namespace SuperMarket.Web.ViewComponents;

public sealed class ProductCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(CustomerProductListItemViewModel? model)
    {
        if (model is null)
            return Content(string.Empty);

        return View(model);
    }
}