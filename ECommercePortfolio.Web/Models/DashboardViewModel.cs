// ECommercePortfolio.Web/Models/DashboardViewModel.cs
namespace ECommercePortfolio.Web.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalInventoryValue { get; set; }
        // Recent activity
                public IEnumerable<ProductListViewModel> RecentProducts { get; set; }
                public IEnumerable<OrderListViewModel> RecentOrders { get; set; }

                // Inventory stats
                public decimal TotalInventoryValue { get; set; }
                public int LowStockItemsCount { get; set; }
                public int OutOfStockItemsCount { get; set; }

                // Charts data
                public Dictionary<string, decimal> SalesByCategory { get; set; }
                public Dictionary<DateTime, decimal> SalesByDate { get; set; }
                public Dictionary<string, int> OrdersByStatus { get; set; }
    }
}