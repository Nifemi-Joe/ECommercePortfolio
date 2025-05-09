// ECommercePortfolio.Web/Controllers/DashboardController.cs
using ECommercePortfolio.Core.Entities;
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ECommercePortfolio.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Define date ranges
            var today = DateTime.Today;
            var startDate = today.AddDays(-30);
            var endDate = today;

            // Get total sales and orders for the last 30 days
            var totalSales = await _unitOfWork.Orders.GetTotalSalesAsync(startDate, endDate);
            var totalOrders = await _unitOfWork.Orders.GetTotalOrdersAsync(startDate, endDate);

            // Get product counts and inventory value
            var totalProducts = await _unitOfWork.Products.GetTotalProductCountAsync();
            var totalInventoryValue = await _unitOfWork.Products.GetTotalInventoryValueAsync();

            // Get monthly sales data for the last 6 months
            var monthlySales = await GetMonthlySalesData(6);

            // Get sales by category
            var categorySales = await GetCategorySalesData();

            // Get latest products
            var latestProducts = await GetLatestProducts(5);

            // Get low stock products
            var lowStockProducts = await GetLowStockProducts(10);

            // Get recent orders
            var recentOrders = await GetRecentOrders(5);

            var viewModel = new DashboardViewModel
            {
                TotalSales = totalSales,
                TotalOrders = totalOrders,
                TotalProducts = totalProducts,
                TotalInventoryValue = totalInventoryValue,
                MonthlySales = monthlySales,
                CategorySales = categorySales,
                LatestProducts = latestProducts,
                LowStockProducts = lowStockProducts,
                RecentOrders = recentOrders
            };

            return View(viewModel);
        }

        private async Task<List<MonthlySalesDto>> GetMonthlySalesData(int months)
        {
            var today = DateTime.Today;
            var result = new List<MonthlySalesDto>();

            // Get all orders within the date range
            var startDate = today.AddMonths(-months + 1).AddDays(-(today.Day - 1)); // Start of the earliest month
            var orders = await _unitOfWork.Orders.GetOrdersByDateRangeAsync(startDate, today);

            // Group by month and calculate totals
            for (int i = 0; i < months; i++)
            {
                var date = today.AddMonths(-i);
                var monthName = date.ToString("MMM");
                var monthYear = date.ToString("MMM yyyy");

                var monthStart = new DateTime(date.Year, date.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var monthlyTotal = orders
                    .Where(o => o.OrderDate >= monthStart && o.OrderDate <= monthEnd)
                    .Sum(o => o.TotalAmount);

                result.Add(new MonthlySalesDto
                {
                    Month = monthName,
                    Amount = monthlyTotal
                });
            }

            // Reverse to get chronological order
            result.Reverse();
            return result;
        }

        private async Task<List<CategorySalesDto>> GetCategorySalesData()
        {
            var result = new List<CategorySalesDto>();
            var categories = await _unitOfWork.Categories.ListAllAsync();

            // For each category, sum up the sales from order items
            foreach (var category in categories)
            {
                var products = await _unitOfWork.Products.GetProductsByCategoryAsync(category.Id);

                decimal categoryTotal = 0;
                foreach (var product in products)
                {
                    // This is a simplified calculation - in a real app you'd use a more efficient query
                    var orderItems = await _unitOfWork.Orders.ListAsync(o =>
                        o.OrderItems.Any(oi => oi.ProductId == product.Id));

                    categoryTotal += orderItems
                        .SelectMany(o => o.OrderItems)
                        .Where(oi => oi.ProductId == product.Id)
                        .Sum(oi => oi.UnitPrice * oi.Quantity);
                }

                result.Add(new CategorySalesDto
                {
                    CategoryName = category.Name,
                    Amount = categoryTotal
                });
            }

            return result.OrderByDescending(c => c.Amount).ToList();
        }

        private async Task<List<ProductDto>> GetLatestProducts(int count)
        {
            var products = await _unitOfWork.Products.GetProductsWithCategoryAsync();

            return products
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category?.Name ?? "Uncategorized",
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive
                })
                .ToList();
        }

        private async Task<List<ProductDto>> GetLowStockProducts(int count)
        {
            var products = await _unitOfWork.Products.GetProductsWithCategoryAsync();

            return products
                .Where(p => p.StockQuantity < 10 && p.IsActive) // Consider products with less than 10 items as low stock
                .OrderBy(p => p.StockQuantity)
                .Take(count)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category?.Name ?? "Uncategorized",
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive
                })
                .ToList();
        }

        private async Task<List<OrderDto>> GetRecentOrders(int count)
        {
            var orders = await _unitOfWork.Orders.ListAllAsync();
            var allUsers = _userManager.Users.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");

            return orders
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    OrderDate = o.OrderDate,
                    CustomerName = o.UserId != null && allUsers.ContainsKey(o.UserId)
                        ? allUsers[o.UserId]
                        : "Guest",
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                })
                .ToList();
        }
    }
}