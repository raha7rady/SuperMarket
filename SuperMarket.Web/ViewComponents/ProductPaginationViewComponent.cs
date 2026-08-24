using Microsoft.AspNetCore.Mvc;
using SuperMarket.Web.Areas.Customer.ViewModels.Products;

namespace SuperMarket.Web.ViewComponents;

public sealed class ProductPaginationViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(CustomerProductPaginationViewModel? model)
    {
        if (model is null)
        {
            model = new CustomerProductPaginationViewModel();
        }

        model.PageNumber = model.PageNumber < 1 ? 1 : model.PageNumber;
        model.PageSize = model.PageSize < 1 ? 12 : model.PageSize;

        return View(model);
    }
}