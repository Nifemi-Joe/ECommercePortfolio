// ECommercePortfolio.Infrastructure/Services/OrderService.cs
using ECommercePortfolio.Core.Entities;
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePortfolio.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _unitOfWork.Orders.ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _unitOfWork.Orders.GetOrderWithItemsAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(string userId)
        {
            return await _unitOfWork.Orders.GetOrdersForUserAsync(userId);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            // Generate a unique order number
            order.OrderNumber = $"ORD-{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}-{Guid.NewGuid().ToString().Substring(0, 4)}";

            // Calculate total amount from order items
            decimal total = 0;
            foreach (var item in order.OrderItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                item.UnitPrice = product.Price;
                total += item.UnitPrice * item.Quantity;

                // Update product stock
                product.StockQuantity -= item.Quantity;
                await _unitOfWork.Products.UpdateAsync(product);
            }

            order.TotalAmount = total;

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            return order;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.Orders.GetOrdersByDateRangeAsync(startDate, endDate);
        }

        public async Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.Orders.GetTotalSalesAsync(startDate, endDate);
        }

        public async Task<int> GetTotalOrdersAsync(DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.Orders.GetTotalOrdersAsync(startDate, endDate);
        }
    }
}