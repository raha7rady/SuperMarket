using Microsoft.AspNetCore.Mvc;
using SuperMarket.Web.Components;

namespace SuperMarket.Web.ViewComponents;

[ViewComponent(Name = "AdminMenuViewComponent")]
public sealed class AdminMenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var currentController = ViewContext.RouteData.Values["controller"]?.ToString();
        var currentAction = ViewContext.RouteData.Values["action"]?.ToString();

        var items = new List<AdminMenuItem>
        {
            new()
            {
                Title = "داشبورد",
                Controller = "Dashboard",
                Action = "Index",
                Icon = "bi-speedometer2"
            },
            new()
            {
                Title = "محصولات",
                Controller = "Products",
                Action = "Index",
                Icon = "bi-box-seam"
            },
            new()
            {
                Title = "دسته‌بندی‌ها",
                Controller = "Categories",
                Action = "Index",
                Icon = "bi-tags"
            },
            new()
            {
                Title = "سفارشات",
                Controller = "Orders",
                Action = "Index",
                Icon = "bi-receipt"
            },
            new()
            {
                Title = "پرداخت‌ها",
                Controller = "Payments",
                Action = "Index",
                Icon = "bi-credit-card"
            },
            new()
            {
                Title = "کاربران",
                Controller = "Users",
                Action = "Index",
                Icon = "bi-people"
            },
            new()
            {
                Title = "خروج",
                Controller = "Account",
                Action = "Logout",
                Icon = "bi-box-arrow-right",
                IsLogout = true
            }
        };

        foreach (var item in items)
        {
            item.IsActive =
                string.Equals(item.Controller, currentController, StringComparison.OrdinalIgnoreCase)
                && string.Equals(item.Action, currentAction, StringComparison.OrdinalIgnoreCase);
        }

        return View(items);
    }
}