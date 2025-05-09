// ECommercePortfolio.Web/Areas/Admin/Controllers/DashboardController.cs
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommercePortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel();

            try
            {
                // Get basic stats
                viewModel.TotalProducts = await _unitOfWork.Products.GetTotalProductCountAsync();
                viewModel.TotalCategories = (await _unitOfWork.Categories.ListAllAsync()).Count;
                viewModel.TotalInventoryValue = await _unitOfWork.Products.GetTotalInventoryValueAsync();

                // Get recent products
                var recentProducts = (await _unitOfWork.Products.GetProductsWithCategoryAsync())
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .ToList();

                viewModel.RecentProducts = recentProducts.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryName = p.Category?.Name,
                    CreatedAt = p.CreatedAt
                }).ToList();

                // Get sales data
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
                var endDate = DateTime.UtcNow;

                viewModel.TotalOrders = await _unitOfWork.Orders.GetTotalOrdersAsync(thirtyDaysAgo, endDate);
                viewModel.TotalSales = await _unitOfWork.Orders.GetTotalSalesAsync(thirtyDaysAgo, endDate);

                // Get recent orders
                var recentOrders = (await _unitOfWork.Orders.GetOrdersByDateRangeAsync(thirtyDaysAgo, endDate))
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToList();

                viewModel.RecentOrders = recentOrders.Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = $"{o.User?.FirstName} {o.User?.LastName}",
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    OrderDate = o.OrderDate
                }).ToList();

                // Get products per category
                var categories = await _unitOfWork.Categories.GetCategoriesWithProductsAsync();
                viewModel.ProductsPerCategory = categories.ToDictionary(
                    c => c.Name,
                    c => c.Products.Count
                );

                // Get sales per day for last 7 days
                var salesPerDay = new Dictionary<DateTime, decimal>();
                for (int i = 6; i >= 0; i--)
                {
                    var date = DateTime.UtcNow.Date.AddDays(-i);
                    var nextDay = date.AddDays(1);
                    var dailySales = await _unitOfWork.Orders.GetTotalSalesAsync(date, nextDay);
                    salesPerDay.Add(date, dailySales);
                }
                viewModel.SalesPerDay = salesPerDay;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                ModelState.AddModelError("", "An error occurred while loading dashboard data.");
            }

            return View(viewModel);
        }
    }
}