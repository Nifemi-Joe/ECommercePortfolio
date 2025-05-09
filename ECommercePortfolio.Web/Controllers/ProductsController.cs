// ECommercePortfolio.Web/Controllers/ProductsController.cs
using ECommercePortfolio.Core.Entities;
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ECommercePortfolio.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const int PageSize = 10;

        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchTerm, int? categoryId, int page = 1)
        {
            // This method is similar to querying data with JavaScript filter/map operations
            var products = await _unitOfWork.Products.GetProductsWithCategoryAsync();

            // Apply filters if provided (similar to JS filter() method)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                products = products.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm)).ToList();
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }

            // Transform to view models (similar to JS map() method)
            var productViewModels = products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                CategoryName = p.Category?.Name
            }).ToList();

            // Pagination
            var totalItems = productViewModels.Count;
            var itemsToDisplay = productViewModels
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Get all categories for filter dropdown
            var categories = await _unitOfWork.Categories.ListAllAsync();
            var categoryViewModels = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            var viewModel = new ProductIndexViewModel
            {
                Products = itemsToDisplay,
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                Categories = categoryViewModels,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = totalItems
                }
            };

            return View(viewModel);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _unitOfWork.Products.GetProductWithCategoryByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CategoryName = product.Category?.Name,
                CategoryId = product.CategoryId,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return View(viewModel);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _unitOfWork.Categories.ListAllAsync();
            var viewModel = new ProductCreateViewModel
            {
                // Convert CategoryViewModel to SelectListItem
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    StockQuantity = viewModel.StockQuantity,
                    CategoryId = viewModel.CategoryId,
                    IsActive = viewModel.IsActive
                };

                // Handle image upload
                if (viewModel.ImageFile != null || viewModel.ProductImage != null)
                {
                    // Use whichever isn't null
                    var imageToUse = viewModel.ImageFile ?? viewModel.ProductImage;
                    if (imageToUse != null)
                    {
                        product.ImageUrl = await SaveImageAsync(imageToUse);
                    }
                }

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CompleteAsync();

                TempData["Success"] = "Product created successfully";
                return RedirectToAction(nameof(Index));
            }

            // If we get here, something failed, so redisplay form
            viewModel.Categories = (await _unitOfWork.Categories.ListAllAsync())
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            return View(viewModel);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var categories = await _unitOfWork.Categories.ListAllAsync();

            var viewModel = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                CurrentImageUrl = product.ImageUrl, // Add this property
                ExistingImageUrl = product.ImageUrl,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == product.CategoryId
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(id);

                    if (product == null)
                    {
                        return NotFound();
                    }

                    product.Name = viewModel.Name;
                    product.Description = viewModel.Description;
                    product.Price = viewModel.Price;
                    product.StockQuantity = viewModel.StockQuantity;
                    product.CategoryId = viewModel.CategoryId;
                    product.IsActive = viewModel.IsActive;

                    // Handle image upload
                    // Handle image upload (modified to check both image properties)
                    if (viewModel.ImageFile != null || viewModel.ProductImage != null)
                    {
                        // Use whichever isn't null
                        var imageToUse = viewModel.ImageFile ?? viewModel.ProductImage;
                        if (imageToUse != null)
                        {
                            // Delete old image if it exists
                            if (!string.IsNullOrEmpty(product.ImageUrl))
                            {
                                DeleteImage(product.ImageUrl);
                            }

                            product.ImageUrl = await SaveImageAsync(imageToUse);
                        }
                    }

                    await _unitOfWork.Products.UpdateAsync(product);
                    await _unitOfWork.CompleteAsync();

                    TempData["Success"] = "Product updated successfully";
                }
                catch (Exception)
                {
                    TempData["Error"] = "An error occurred while updating the product";
                }

                return RedirectToAction(nameof(Index));
            }

            // If we get here, something failed, so redisplay form
            viewModel.Categories = (await _unitOfWork.Categories.ListAllAsync())
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == viewModel.CategoryId
                }).ToList();

            return View(viewModel);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Products.GetProductWithCategoryByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CategoryName = product.Category?.Name
            };

            return View(viewModel);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Delete product image if exists
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                DeleteImage(product.ImageUrl);
            }

            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Product deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImageAsync(Microsoft.AspNetCore.Http.IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate unique filename
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/images/products/" + uniqueFileName;
        }

        private void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return;
            }

            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
