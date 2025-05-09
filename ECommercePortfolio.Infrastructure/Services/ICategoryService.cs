// ECommercePortfolio.Core/Services/ICategoryService.cs
using ECommercePortfolio.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePortfolio.Core.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
    }
}
