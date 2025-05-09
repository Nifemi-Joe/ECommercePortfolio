// ECommercePortfolio.Infrastructure/Repositories/CategoryRepository.cs
using ECommercePortfolio.Core.Entities;
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePortfolio.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Category>> GetCategoriesWithProductsAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category> GetCategoryWithProductsByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}

