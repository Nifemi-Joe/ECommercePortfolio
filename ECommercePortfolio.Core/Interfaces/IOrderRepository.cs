// ECommercePortfolio.Core/Interfaces/IOrderRepository.cs
using ECommercePortfolio.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePortfolio.Core.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetOrderWithItemsAsync(int id);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string userId);
        Task<IReadOnlyList<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate);
        Task<int> GetTotalOrdersAsync(DateTime startDate, DateTime endDate);
    }
}