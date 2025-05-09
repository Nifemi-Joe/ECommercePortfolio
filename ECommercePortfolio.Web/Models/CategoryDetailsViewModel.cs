using System.ComponentModel.DataAnnotations;

namespace ECommercePortfolio.Web.Models
{
 public class CategoryDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; } // Nullable DateTime
        public DateTime? UpdatedAt { get; set; } // Nullable DateTime
        public IEnumerable<ProductListViewModel> Products { get; set; }
    }
}