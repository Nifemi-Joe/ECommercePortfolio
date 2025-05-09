// ECommercePortfolio.Core/Interfaces/IUnitOfWork.cs
using System;
using System.Threading.Tasks;

namespace ECommercePortfolio.Core.Interfaces
{
    // The Unit of Work pattern manages multiple repository operations in a single transaction
    // Similar to batching multiple database operations in JavaScript
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IOrderRepository Orders { get; }
        Task<int> CompleteAsync();
        Task<int> UpdateAsync<T>(T entity) where T : class;
        Task<int> DeleteAsync<T>(T entity) where T : class;

                // Include other existing methods your interface already has
        Task<int> SaveChangesAsync();
    }
}