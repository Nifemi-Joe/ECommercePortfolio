// ECommercePortfolio.Infrastructure/Services/ProductService.cs
using ECommercePortfolio.Core.Entities;
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommercePortfolio.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.Products.GetProductsWithCategoryAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.Products.GetProductWithCategoryByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                await _unitOfWork.Products.DeleteAsync(product);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<int> GetTotalProductCountAsync()
        {
            return await _unitOfWork.Products.GetTotalProductCountAsync();
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            return await _unitOfWork.Products.GetTotalInventoryValueAsync();
        }
    }
}