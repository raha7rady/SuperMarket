
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SuperMarket.Application.Common.Interfaces;
using SuperMarket.Domain.Entities;
using SuperMarket.Domain.Enums;
using SuperMarket.Domain.Interfaces.Repositories;
using SuperMarket.Infrastructure.Persistence;
using SuperMarket.Application.Common.Extensions;


namespace SuperMarket.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider services,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        using var scope = services.CreateScope();

        var provider = scope.ServiceProvider;

        var dbContext =
            provider.GetRequiredService<SuperMarketDbContext>();

        var roleManager =
            provider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var userManager =
            provider.GetRequiredService<UserManager<ApplicationUser>>();

        await dbContext.Database.MigrateAsync(cancellationToken);

        await SeedRolesAsync(
            roleManager);

        await SeedSuperAdminAsync(
            userManager,
            configuration,
            provider,
            cancellationToken);
    }

    private static async Task SeedRolesAsync(
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var role in Enum
                     .GetValues<UserRole>()
                     .Where(r => r != UserRole.None))
        {
            var roleName = role.ToString();

            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(
                new IdentityRole<Guid>(roleName));

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create role '{roleName}'. " +
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }
        }
    }

    private static async Task SeedSuperAdminAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        var email =
            configuration["Identity:SuperAdmin:Email"];

        var password =
            configuration["Identity:SuperAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        email = email.Trim().ToLowerInvariant();

        var passwordHasher =
            services.GetRequiredService<IPasswordHasher>();

        var userRepository =
            services.GetRequiredService<IUserRepository>();

        var unitOfWork =
            services.GetRequiredService<IUnitOfWork>();

        // اگر Domain User وجود دارد، Seeder دوباره اجرا نشود.
        if (await userRepository.ExistsByEmailAsync(
                email,
                cancellationToken: cancellationToken))
        {
            return;
        }

        var passwordHash =
            passwordHasher.HashPassword(password);

        var domainUser = new User(
            firstName: "Super",
            lastName: "Admin",
            email: email,
            passwordHash: passwordHash);

        domainUser.ChangeRole(UserRole.SuperAdmin);

        await userRepository.AddAsync(
            domainUser,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        // اگر Identity User از قبل وجود داشته باشد
        var identityUser =
            await userManager.FindByEmailAsync(email);

        if (identityUser is null)
        {
            identityUser = ApplicationUser.Create(
                domainUser.Id,
                email);

            var createResult =
                await userManager.CreateAsync(
                    identityUser,
                    password);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to create SuperAdmin identity user. " +
                    string.Join(", ",
                        createResult.Errors
                            .Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(
                identityUser,
                UserRole.SuperAdmin.ToRoleName()))
        {
            var roleResult =
                await userManager.AddToRoleAsync(
                    identityUser,
                    UserRole.SuperAdmin.ToRoleName());

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to assign SuperAdmin role. " +
                    string.Join(", ",
                        roleResult.Errors
                            .Select(e => e.Description)));
            }
        }
    }
}

