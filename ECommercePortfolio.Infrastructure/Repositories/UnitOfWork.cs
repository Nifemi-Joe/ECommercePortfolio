// ECommercePortfolio.Infrastructure/Repositories/UnitOfWork.cs
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Infrastructure.Data;
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

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}