namespace SuperMarket.Web.Authorization
{
    /// <summary>
    /// نام‌های ثابت برای پالیسی‌های احراز هویت (Authorization Policies)
    /// </summary>
    public static class PolicyNames
    {
        // پالیسی‌های مبتنی بر نقش (Role-Based)
        public const string RequireAdminRole = "RequireAdminRole";
        public const string RequireSuperAdminRole = "RequireSuperAdminRole";
        public const string RequireCustomerRole = "RequireCustomerRole";

        // پالیسی‌های پیشرفته (Claim-Based یا شرطی)
        public const string CanDeleteProduct = "CanDeleteProduct";
    }
}