// ECommercePortfolio.Core/Services/IDashboardService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePortfolio.Core.Services
{
    public interface IDashboardService
    {
        Task<Dictionary<string, decimal>> GetSalesOverviewAsync();
        Task<Dictionary<string, int>> GetProductsByCategory();
        Task<Dictionary<DateTime, decimal>> GetSalesTrendAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, decimal>> GetTopSellingProductsAsync(int count = 5);
        Task<Dictionary<string, object>> GetDashboardSummaryAsync();
    }
}
