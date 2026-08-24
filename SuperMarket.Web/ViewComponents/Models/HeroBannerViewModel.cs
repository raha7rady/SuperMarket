namespace SuperMarket.Web.ViewComponents.Models;

public sealed class HeroBannerViewModel
{
    public string Title { get; init; } = "خرید آنلاین مواد غذایی";
    public string Subtitle { get; init; } = "بهترین محصولات با بهترین قیمت";
    public string ButtonText { get; init; } = "مشاهده محصولات";
    public string ButtonUrl { get; init; } = "/Customer/Product";
}