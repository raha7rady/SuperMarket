// -------------------------
// IdentityConfiguration.cs
// -------------------------
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SuperMarket.Infrastructure.Persistence;

namespace SuperMarket.Infrastructure.Identity;

public static class IdentityConfiguration
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.User.RequireUniqueEmail = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                // Email confirmation UI/flow already exists (ConfirmEmail /
                // ResendConfirmationEmail), but RegisterAsync does not yet
                // send a confirmation email and always reports
                // RequiresEmailConfirmation = false. Keeping this disabled
                // until that flow is completed in the Application layer
                // avoids locking users out with a requirement the app
                // cannot yet fulfill. Revisit when IAccountService gains
                // ConfirmEmailAsync / ResendConfirmationEmailAsync.
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddRoles<IdentityRole<Guid>>() // تضمین ثبت RoleManager
            .AddEntityFrameworkStores<SuperMarketDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
