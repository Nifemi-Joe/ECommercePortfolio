using System.ComponentModel.DataAnnotations;

namespace ECommercePortfolio.Web.Models
{
public class CategoryIndexViewModel
    {
        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public string SearchTerm { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}