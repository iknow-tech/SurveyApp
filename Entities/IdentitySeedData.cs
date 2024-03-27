using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SurveyApp.Entities
{
    public class IdentitySeedData
    {
        private const string adminUser = "admin";

        private const string adminPassword = "Admin_2024";

        public static async void IdentityTestUser(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Database.GetAppliedMigrations().Any())
            {
                context.Database.Migrate();
            }

            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            if (roleManager.Roles.ToList().Count == 0)
            {
                var roles = new List<string>() { "ADMIN", "AUDITOR", "EMPLOYEE" };

                foreach (var item in roles)
                {
                    var appRole = new AppRole()
                    {
                        Name = item
                    };

                    await roleManager.CreateAsync(appRole);
                }

                var user = await userManager.FindByNameAsync(adminUser);

                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = adminUser,
                        FullName = "iKnow Admin",
                        Email = "admin@iknow.com",
                        PhoneNumber = "123123123"
                    };

                    var result = await userManager.CreateAsync(user, adminPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "ADMIN");
                    }
                }
            }
        }
    }
}
