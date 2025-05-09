// ECommercePortfolio.Web/Areas/Admin/Models/DashboardViewModel.cs
using System;
using System.Collections.Generic;

namespace ECommercePortfolio.Web.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int MonthlyOrders { get; set; }
        public decimal MonthlySales { get; set; }

        // Could be expanded with additional dashboard metrics
        public List<KeyValuePair<string, int>> ProductsByCategory { get; set; } = new List<KeyValuePair<string, int>>();
        public List<KeyValuePair<DateTime, decimal>> SalesByDay { get; set; } = new List<KeyValuePair<DateTime, decimal>>();
    }
}
