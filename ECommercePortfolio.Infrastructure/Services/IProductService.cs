// ECommercePortfolio.Core/Services/IProductService.cs
using ECommercePortfolio.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePortfolio.Core.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<int> GetTotalProductCountAsync();
        Task<decimal> GetTotalInventoryValueAsync();
    }
}
