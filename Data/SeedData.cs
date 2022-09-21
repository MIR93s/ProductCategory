using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductCategories.Authorization;
using ProductCategories.Models;

namespace ProductCategories.Data
{
    public class SeedData
    {
        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.Category.Any())
            {
                return; // DB has been seeded
            }
            context.Category.AddRange(
            new Category
            {
                Title = "Clothes"
            },
            new Category
            {
                Title = "Drink"
            },
            new Category
            {
                Title = "Sport"
            },
             new Category
             {
                 Title = "Vegetable"
             },
            new Category
            {
                Title = "Meat"
            },
            new Category
            {
                Title = "Hygiene"
            },
            new Category
            {
                Title = "Bread"
            });
            context.SaveChanges();
        } 
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything
                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@product.com");
                await EnsureRole(serviceProvider, adminID, Constants.AdministratorsRole);
                SeedDB(context, adminID);
            }
        }
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
        string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }
            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }
            return user.Id;
        }
        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
        string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }
            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            //if (userManager == null)
            //{
            // throw new Exception("userManager is null");
            //}
            var user = await userManager.FindByIdAsync(uid);
            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }
            IR = await userManager.AddToRoleAsync(user, role);
            return IR;
        }
    }
}
