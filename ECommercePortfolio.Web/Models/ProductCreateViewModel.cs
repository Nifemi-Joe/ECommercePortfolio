// ECommercePortfolio.Web/Models/ProductCreateViewModel.cs
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommercePortfolio.Web.Models
{
    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between $0.01 and $10,000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a positive number")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }

        [Display(Name = "Product Image")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}