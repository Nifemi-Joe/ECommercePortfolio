// ECommercePortfolio.Web/Areas/Admin/Models/ProductViewModel.cs
using ECommercePortfolio.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ECommercePortfolio.Web.Areas.Admin.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, 10000.00)]
        [Display(Name = "Price ($)")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 10000)]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        // Image handling
        [Display(Name = "Product Image")]
        public IFormFile ImageFile { get; set; }

        public string ExistingImageUrl { get; set; }
    }
}