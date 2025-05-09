// ECommercePortfolio.Infrastructure/Services/CategoryService.cs
using ECommercePortfolio.Core.Entities;
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePortfolio.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.Categories.ListAllAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _unitOfWork.Categories.GetCategoryWithProductsByIdAsync(id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();
            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category != null)
            {
                await _unitOfWork.Categories.DeleteAsync(category);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
