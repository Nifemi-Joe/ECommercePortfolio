// ECommercePortfolio.Infrastructure/Repositories/ProductRepository.cs
using ECommercePortfolio.Core.Entities;
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommercePortfolio.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Product>> GetProductsWithCategoryAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product> GetProductWithCategoryByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetTotalProductCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            return await _context.Products.SumAsync(p => p.Price * p.StockQuantity);
        }
    }
}