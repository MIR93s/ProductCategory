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
            //context.Database.EnsureCreated();
            if (context.Category.Any())
            {
                return; // DB has been seeded
            }
            var categories  = new Category[] {
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
                     Title = "Dairy"
                 },
                new Category
                {
                    Title = "Bakery"
                } 
            };

            foreach (var item in categories)
            {
                context.Category.Add(item);
            }
            context.SaveChanges();

            var products = new Models.Product[] {
                new Models.Product
                {
                    Name ="Cheese"
                },
                new Models.Product
                {
                     Name = "Milk"
                },
                new Models.Product
                {
                     Name = "Tometo"
                },
                new Models.Product
                {
                     Name = "beef"
                },
                new Models.Product
                {
                    Name = "Bread"
                },
                new Models.Product
                {
                    Name = "Shoes"
                },
                new Models.Product
                {
                    Name = "Shampoo"
                }
            };
            foreach (var item in products)
            {
                context.Product.Add(item);
            }
            context.SaveChanges();

            var productCategories = new ProductCategory[] {
                new ProductCategory
                {
                ProductId = products.Single(p => p.Name == "Cheese").Id,
                CategoryId = categories.Single(c => c.Title == "Dairy").Id
                },
                new ProductCategory
                {
                ProductId = products.Single(p => p.Name == "Milk").Id,
                CategoryId = categories.Single(c => c.Title == "Dairy").Id
                },
                new ProductCategory
                {
                ProductId = products.Single(p => p.Name == "Milk").Id,
                CategoryId = categories.Single(c => c.Title == "Drink").Id
                },
                new ProductCategory
                {
                ProductId = products.Single(p => p.Name == "Tometo").Id,
                CategoryId = categories.Single(c => c.Title == "Vegetable").Id
                },
                new ProductCategory
                {
                ProductId = products.Single(p => p.Name == "beef").Id,
                CategoryId = categories.Single(c => c.Title == "Meat").Id
                },
                new ProductCategory
                {
                    ProductId = products.Single(p => p.Name == "Bread").Id,
                    CategoryId = categories.Single(c => c.Title == "Bakery").Id
                },
                new ProductCategory
                {
                    ProductId = products.Single(p => p.Name == "Shoes").Id,
                    CategoryId = categories.Single(c => c.Title == "Sport").Id
                },
                  new ProductCategory
                  {
                      ProductId = products.Single(p => p.Name == "Shoes").Id,
                      CategoryId = categories.Single(c => c.Title == "Clothes").Id
                  },
                    new ProductCategory
                    {
                        ProductId = products.Single(p => p.Name == "Shampoo").Id,
                        CategoryId = categories.Single(c => c.Title == "Hygiene").Id
                    } };
            foreach (var item in productCategories)
            {
                context.ProductCategory.Add(item);
            }
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
