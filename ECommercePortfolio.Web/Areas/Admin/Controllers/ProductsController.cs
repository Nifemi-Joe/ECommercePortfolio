// ECommercePortfolio.Web/Areas/Admin/Controllers/ProductsController.cs
using ECommercePortfolio.Core.Entities;
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ECommercePortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Products.GetProductsWithCategoryAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _unitOfWork.Products.GetProductWithCategoryByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _unitOfWork.Categories.ListAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                string uniqueFileName = null;
                if (productViewModel.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "products");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + productViewModel.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productViewModel.ImageFile.CopyToAsync(fileStream);
                    }
                }

                // Create product entity
                var product = new Product
                {
                    Name = productViewModel.Name,
                    Description = productViewModel.Description,
                    Price = productViewModel.Price,
                    StockQuantity = productViewModel.StockQuantity,
                    CategoryId = productViewModel.CategoryId,
                    IsActive = productViewModel.IsActive,
                    ImageUrl = uniqueFileName != null ? $"/images/products/{uniqueFileName}" : "/images/products/default.jpg"
                };

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed, redisplay form
            var categories = await _unitOfWork.Categories.ListAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", productViewModel.CategoryId);
            return View(productViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productViewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                ExistingImageUrl = product.ImageUrl
            };

            var categories = await _unitOfWork.Categories.ListAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel productViewModel)
        {
            if (id != productViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Handle image update
                string uniqueFileName = null;
                if (productViewModel.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "products");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + productViewModel.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productViewModel.ImageFile.CopyToAsync(fileStream);
                    }

                    // Delete old image if it exists and is not the default
                    if (!string.IsNullOrEmpty(product.ImageUrl) &&
                        !product.ImageUrl.EndsWith("default.jpg") &&
                        System.IO.File.Exists(Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'))))
                    {
                        System.IO.File.Delete(Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/')));
                    }
                }

                // Update product entity
                product.Name = productViewModel.Name;
                product.Description = productViewModel.Description;
                product.Price = productViewModel.Price;
                product.StockQuantity = productViewModel.StockQuantity;
                product.CategoryId = productViewModel.CategoryId;
                product.IsActive = productViewModel.IsActive;

                if (uniqueFileName != null)
                {
                    product.ImageUrl = $"/images/products/{uniqueFileName}";
                }

                await _unitOfWork.UpdateAsync(product);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed, redisplay form
            var categories = await _unitOfWork.Categories.ListAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", productViewModel.CategoryId);
            return View(productViewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Products.GetProductWithCategoryByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Delete image if it exists and is not the default
            if (!string.IsNullOrEmpty(product.ImageUrl) &&
                !product.ImageUrl.EndsWith("default.jpg") &&
                System.IO.File.Exists(Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'))))
            {
                System.IO.File.Delete(Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/')));
            }

            await _unitOfWork.DeleteAsync(product);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
