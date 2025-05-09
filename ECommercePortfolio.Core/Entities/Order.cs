
// ECommercePortfolio.Core/Entities/Order.cs
using System;
using System.Collections.Generic;

namespace ECommercePortfolio.Core.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus
    {
        Pending = 0,
        Processing = 1,
        Shipped = 2,
        Delivered = 3,
        Cancelled = 4
    }
}