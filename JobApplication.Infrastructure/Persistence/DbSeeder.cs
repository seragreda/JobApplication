using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace JobApplication.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = sp.GetRequiredService<UserManager<AppUser>>();
        var uow = sp.GetRequiredService<IUnitOfWork>();

        // Roles
        foreach (var role in new[] { AppRoles.Recruiter, AppRoles.Candidate })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Default Recruiter
        const string recruiterEmail = "recruiter@test.com";
        if (await userManager.FindByEmailAsync(recruiterEmail) is null)
        {
            var user = new AppUser
            {
                UserName = recruiterEmail,
                Email = recruiterEmail,
                FullName = "Default Recruiter"
            };
            await userManager.CreateAsync(user, "Recruiter@123");
            await userManager.AddToRoleAsync(user, AppRoles.Recruiter);

            await uow.Recruiters.AddAsync(new Recruiter
            {
                Name = "Default Recruiter",
                CompanyName = "Demo Co.",
                UserId = user.Id
            });
            await uow.SaveChangesAsync();
        }
    }
}