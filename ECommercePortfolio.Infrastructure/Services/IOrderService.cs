// ECommercePortfolio.Core/Services/IOrderService.cs
using ECommercePortfolio.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePortfolio.Core.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrdersForUserAsync(string userId);
        Task<Order> CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate);
        Task<int> GetTotalOrdersAsync(DateTime startDate, DateTime endDate);
    }
}
