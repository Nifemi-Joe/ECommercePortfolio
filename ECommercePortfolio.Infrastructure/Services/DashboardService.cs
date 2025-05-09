// ECommercePortfolio.Infrastructure/Services/DashboardService.cs
using ECommercePortfolio.Core.Entities;
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Core.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommercePortfolio.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public DashboardService(
            IUnitOfWork unitOfWork,
            IProductService productService,
            IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
            _orderService = orderService;
        }

        public async Task<Dictionary<string, decimal>> GetSalesOverviewAsync()
        {
            // Get sales for different time periods
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);
            var lastWeekStart = today.AddDays(-7);
            var lastMonthStart = today.AddMonths(-1);
            var lastYearStart = today.AddYears(-1);

            // Calculate sales for different periods
            var todaySales = await _orderService.GetTotalSalesAsync(today, today.AddDays(1).AddTicks(-1));
            var yesterdaySales = await _orderService.GetTotalSalesAsync(yesterday, yesterday.AddDays(1).AddTicks(-1));
            var lastWeekSales = await _orderService.GetTotalSalesAsync(lastWeekStart, today.AddDays(1).AddTicks(-1));
            var lastMonthSales = await _orderService.GetTotalSalesAsync(lastMonthStart, today.AddDays(1).AddTicks(-1));
            var lastYearSales = await _orderService.GetTotalSalesAsync(lastYearStart, today.AddDays(1).AddTicks(-1));

            return new Dictionary<string, decimal>
            {
                { "Today", todaySales },
                { "Yesterday", yesterdaySales },
                { "Last 7 Days", lastWeekSales },
                { "Last 30 Days", lastMonthSales },
                { "Last 365 Days", lastYearSales }
            };
        }

        public async Task<Dictionary<string, int>> GetProductsByCategory()
        {
            var categories = await _unitOfWork.Categories.GetCategoriesWithProductsAsync();
            return categories.ToDictionary(c => c.Name, c => c.Products.Count);
        }

        public async Task<Dictionary<DateTime, decimal>> GetSalesTrendAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByDateRangeAsync(startDate, endDate);

            // Group orders by date and calculate total sales for each date
            var salesByDate = orders
                .GroupBy(o => o.OrderDate.Date)
                .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalAmount));

            // Ensure all dates in the range are included
            var result = new Dictionary<DateTime, decimal>();
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (salesByDate.ContainsKey(date))
                {
                    result[date] = salesByDate[date];
                }
                else
                {
                    result[date] = 0;
                }
            }

            return result;
        }

        public async Task<Dictionary<string, decimal>> GetTopSellingProductsAsync(int count = 5)
        {
            var result = new Dictionary<string, decimal>();

            // Get all orders with items
            var orders = await Task.WhenAll(
                (await _orderService.GetAllOrdersAsync())
                .Select(async o => await _orderService.GetOrderByIdAsync(o.Id))
            );

            // Group by product and calculate total quantity sold
            var topProducts = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(i => i.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.UnitPrice * i.Quantity)
                })
                .OrderByDescending(x => x.Revenue)
                .Take(count);

            // Get product names for the IDs
            foreach (var item in topProducts)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                result[product.Name] = item.Revenue;
            }

            return result;
        }

        public async Task<Dictionary<string, object>> GetDashboardSummaryAsync()
        {
            var today = DateTime.UtcNow.Date;
            var lastMonthStart = today.AddMonths(-1);

            // Get various metrics
            var totalProducts = await _productService.GetTotalProductCountAsync();
            var totalInventoryValue = await _productService.GetTotalInventoryValueAsync();
            var monthlyOrders = await _orderService.GetTotalOrdersAsync(lastMonthStart, today.AddDays(1).AddTicks(-1));
            var monthlySales = await _orderService.GetTotalSalesAsync(lastMonthStart, today.AddDays(1).AddTicks(-1));
            var topSellingProducts = await GetTopSellingProductsAsync(5);
            var categoryDistribution = await GetProductsByCategory();

            return new Dictionary<string, object>
            {
                { "TotalProducts", totalProducts },
                { "TotalInventoryValue", totalInventoryValue },
                { "MonthlyOrders", monthlyOrders },
                { "MonthlySales", monthlySales },
                { "TopSellingProducts", topSellingProducts },
                { "CategoryDistribution", categoryDistribution }
            };
        }
    }
}