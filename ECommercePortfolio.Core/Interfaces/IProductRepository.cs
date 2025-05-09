// ECommercePortfolio.Core/Interfaces/IProductRepository.cs
using ECommercePortfolio.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePortfolio.Core.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IReadOnlyList<Product>> GetProductsWithCategoryAsync();
        Task<Product> GetProductWithCategoryByIdAsync(int id);
        Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<int> GetTotalProductCountAsync();
        Task<decimal> GetTotalInventoryValueAsync();
    }
}