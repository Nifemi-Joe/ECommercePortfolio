// ECommercePortfolio.Core/Interfaces/ICategoryRepository.cs
using ECommercePortfolio.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePortfolio.Core.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IReadOnlyList<Category>> GetCategoriesWithProductsAsync();
        Task<Category> GetCategoryWithProductsByIdAsync(int id);
    }
}