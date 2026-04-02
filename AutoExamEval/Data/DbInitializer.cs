using AutoExamEval.Entities;
using Microsoft.AspNetCore.Identity;

namespace AutoExamEval.Data;

public static class DbInitializer
{
    private const string AdminRole = "Admin";
    private const string InstructorRole = "Instructor";

    private const string AdminEmail = "admin@sinavsistemi.com";
    private const string AdminPassword = "Admin123*";
    private const string AdminFullName = "System Administrator";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        await EnsureRoleExistsAsync(roleManager, AdminRole);
        await EnsureRoleExistsAsync(roleManager, InstructorRole);

        var adminUser = await userManager.FindByEmailAsync(AdminEmail);
        if (adminUser is null)
        {
            adminUser = new AppUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                FullName = AdminFullName,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, AdminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Admin user could not be seeded: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, AdminRole))
        {
            var addRoleResult = await userManager.AddToRoleAsync(adminUser, AdminRole);
            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Admin role assignment failed: {errors}");
            }
        }
    }

    private static async Task EnsureRoleExistsAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Role '{roleName}' could not be created: {errors}");
            }
        }
    }
}
