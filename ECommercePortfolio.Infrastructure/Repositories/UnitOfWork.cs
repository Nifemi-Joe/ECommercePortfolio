using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ECommercePortfolio.Infrastructure.Repositories
{
    // Unit of Work pattern - manages multiple repository operations in a single transaction
    // Similar to transaction management in JavaScript database operations
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IProductRepository _productRepository;
        private ICategoryRepository _categoryRepository;
        private IOrderRepository _orderRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);

        public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context);

        public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Implementation for the SaveChangesAsync method
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Implementation for UpdateAsync method
        public async Task<int> UpdateAsync<T>(T entity) where T : class
        {
            _context.Entry(entity).State = EntityState.Modified;
            return await SaveChangesAsync();
        }

        // Implementation for DeleteAsync method
        public async Task<int> DeleteAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
            return await SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}