using ECommercePortfolio.Core.Entities;
using System;
using System.Collections.Generic;

namespace ECommercePortfolio.Web.Models
{
    public class DashboardViewModel
    {
        // Summary metrics
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }

        // Chart data - Fixed property names to match what's used in the view
        public IEnumerable<MonthlySalesDto> MonthlySales { get; set; } = new List<MonthlySalesDto>();
        public IEnumerable<CategorySalesDto> CategorySales { get; set; } = new List<CategorySalesDto>();

        // These properties were missing but referenced in the view
        public IEnumerable<MonthlySalesDto> MonthlySalesData { get; set; } = new List<MonthlySalesDto>();
        public IEnumerable<CategorySalesDto> CategorySalesData { get; set; } = new List<CategorySalesDto>();
        public IEnumerable<OrderDto> MonthlyOrders { get; set; } = new List<OrderDto>();

        // Table data
        public IEnumerable<ProductDto> LatestProducts { get; set; } = new List<ProductDto>();
        public IEnumerable<ProductDto> LowStockProducts { get; set; } = new List<ProductDto>();
        public IEnumerable<OrderDto> RecentOrders { get; set; } = new List<OrderDto>();
    }

    public class MonthlySalesDto
    {
        public string Month { get; set; }
        public decimal Amount { get; set; }
    }

    public class CategorySalesDto
    {
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
    }
}