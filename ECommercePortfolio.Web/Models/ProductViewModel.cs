// ECommercePortfolio.Web/Models/ProductViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace ECommercePortfolio.Web.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between $0.01 and $10,000")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, 10000, ErrorMessage = "Stock quantity must be between 0 and 10,000")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Display(Name = "Image URL")]
        [StringLength(300)]
        public string ImageUrl { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime? UpdatedAt { get; set; }
    }
}
