
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SuperMarket.Web.Authorization;

namespace SuperMarket.Web.Extensions
{
    public static class AuthorizationExtensions
    {
        public static void AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                #region 1. پالیسی‌های مبتنی بر نقش (Role-Based)

                // ادمین‌ها و سوپرادمین‌ها می‌توانند وارد پنل ادمین شوند
                options.AddPolicy(PolicyNames.RequireAdminRole, policy =>
                    policy.RequireRole("Admin", "SuperAdmin"));

                // فقط سوپرادمین‌ها دسترسی‌های حساس دارند (مثلاً مدیریت کاربران دیگر)
                options.AddPolicy(PolicyNames.RequireSuperAdminRole, policy =>
                    policy.RequireRole("SuperAdmin"));

                // فقط مشتریان (اختیاری، معمولاً برای بخش Customer استفاده می‌شود)
                options.AddPolicy(PolicyNames.RequireCustomerRole, policy =>
                    policy.RequireRole("Customer"));

                #endregion

                #region 2. پالیسی‌های پیشرفته (Advanced/Claim-Based)

                // مثال: فقط ادمین‌ها و سوپرادمین‌ها حق حذف محصول را دارند
                options.AddPolicy(PolicyNames.CanDeleteProduct, policy =>
                    policy.RequireRole("Admin", "SuperAdmin"));

                #endregion
            });
        }
    }
}