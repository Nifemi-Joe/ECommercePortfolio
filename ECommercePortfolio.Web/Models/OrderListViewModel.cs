// ECommercePortfolio.Web/Models/OrderListViewModel.cs
using ECommercePortfolio.Core.Entities;
using System;

namespace ECommercePortfolio.Web.Models
{
    public class OrderListViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string UserName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public int ItemCount { get; set; }
    }
}
