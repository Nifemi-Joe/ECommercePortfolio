// ECommercePortfolio.Web/Models/CategoryViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace ECommercePortfolio.Web.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }

        public int ProductCount { get; set; }
    }
}