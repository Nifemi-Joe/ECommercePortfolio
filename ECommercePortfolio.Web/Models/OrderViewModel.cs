// ECommercePortfolio.Web/Models/OrderViewModel.cs
using ECommercePortfolio.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace ECommercePortfolio.Web.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Status")]
        public OrderStatus Status { get; set; }

        [Display(Name = "Order Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        public DateTime OrderDate { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        public decimal Total => UnitPrice * Quantity;
    }
}
