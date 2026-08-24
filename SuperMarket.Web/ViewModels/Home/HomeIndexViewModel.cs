using SuperMarket.Web.Areas.Customer.ViewModels.Categories;
using SuperMarket.Web.Areas.Customer.ViewModels.Products;

namespace SuperMarket.Web.ViewModels.Home;

public sealed class HomeIndexViewModel
{
    public IReadOnlyList<CustomerProductCardViewModel> FeaturedProducts { get; init; }
        = [];

    public IReadOnlyList<CustomerCategoryItemViewModel> Categories { get; init; }
        = [];

    public string HeroTitle { get; init; }
        = "به سوپرمارکت آنلاین خوش آمدید";

    public string HeroDescription { get; init; }
        = "خرید سریع، آسان و مطمئن";
}