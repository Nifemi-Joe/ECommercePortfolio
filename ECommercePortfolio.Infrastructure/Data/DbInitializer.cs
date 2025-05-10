// ECommercePortfolio.Infrastructure/Data/DbInitializer.cs
using ECommercePortfolio.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommercePortfolio.Infrastructure.Data
{
    // Similar to database seeding in JavaScript applications
    public class DbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DbInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Apply any pending migrations
                await _context.Database.MigrateAsync();

                // Create roles if they don't exist
                await SeedRolesAsync();

                // Create admin user if it doesn't exist
                await SeedAdminUserAsync();

                // Seed categories and products if they don't exist
                await SeedCategoriesAndProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private async Task SeedRolesAsync()
        {
            var roles = new[] { "Admin", "Manager", "Customer" };

            foreach(var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation($"Created role: {role}");
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminEmail = "admin@example.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User"
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin@123456");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation($"Created admin user: {adminEmail}");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"Could not create admin user. Errors: {errors}");
                }
            }
        }

        private async Task SeedCategoriesAndProductsAsync()
        {
            if (!_context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Electronics", Description = "Electronic devices and accessories" },
                    new Category { Name = "Clothing", Description = "Apparel and fashion items" },
                    new Category { Name = "Home & Kitchen", Description = "Household and kitchen items" },
                    new Category { Name = "Books", Description = "Books and publications" },
                    new Category { Name = "Sports & Outdoors", Description = "Sports equipment and outdoor gear" }
                };

                await _context.Categories.AddRangeAsync(categories);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Seeded categories");

                // Add some sample products
                var products = new List<Product>
                {
                    new Product {
                        Name = "Smartphone",
                        Description = "Latest smartphone with advanced features",
                        Price = 699.99m,
                        StockQuantity = 100,
                        CategoryId = categories[0].Id,
                        ImageUrl = ""
                    },
                    new Product {
                        Name = "Laptop",
                        Description = "High-performance laptop for professionals",
                        Price = 1299.99m,
                        StockQuantity = 50,
                        CategoryId = categories[0].Id,
                        ImageUrl = ""
                    },
                    new Product {
                        Name = "T-Shirt",
                        Description = "Cotton t-shirt, available in multiple colors",
                        Price = 19.99m,
                        StockQuantity = 200,
                        CategoryId = categories[1].Id,
                        ImageUrl = ""
                    },
                    new Product {
                        Name = "Coffee Maker",
                        Description = "Automatic coffee maker with timer",
                        Price = 89.99m,
                        StockQuantity = 75,
                        CategoryId = categories[2].Id,
                        ImageUrl = ""
                    },
                    new Product {
                        Name = "Novel",
                        Description = "Bestselling fiction novel",
                        Price = 24.99m,
                        StockQuantity = 150,
                        CategoryId = categories[3].Id,
                        ImageUrl = ""
                    },
                    new Product {
                        Name = "Yoga Mat",
                        Description = "Non-slip yoga mat for exercise",
                        Price = 29.99m,
                        StockQuantity = 100,
                        CategoryId = categories[4].Id,
                        ImageUrl = ""
                    }
                };

                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Seeded products");
            }
        }
    }
}