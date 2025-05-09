// ECommercePortfolio.Web/Models/ProductIndexViewModel.cs
using System.Collections.Generic;

namespace ECommercePortfolio.Web.Models
{
    public class ProductIndexViewModel
    {
        public IEnumerable<ProductListViewModel> Products { get; set; }
        public string SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}