// ECommercePortfolio.Web/Models/ProductListViewModel.cs
namespace ECommercePortfolio.Web.Models
{
    public class ProductListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; }
    }
}
