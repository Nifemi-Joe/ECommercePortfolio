// ECommercePortfolio.Web/Controllers/CategoriesController.cs
using ECommercePortfolio.Core.Entities;
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ECommercePortfolio.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int PageSize = 10;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Categories
        public async Task<IActionResult> Index(string searchTerm, int page = 1)
        {
            var categories = await _unitOfWork.Categories.GetCategoriesWithProductsAsync();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                categories = categories.Where(c =>
                    c.Name.ToLower().Contains(searchTerm) ||
                    c.Description?.ToLower().Contains(searchTerm) == true).ToList();
            }

            // Transform to view models
            var categoryViewModels = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = c.Products.Count
            }).ToList();

            // Pagination
            var totalItems = categoryViewModels.Count;
            var itemsToDisplay = categoryViewModels
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var viewModel = new CategoryIndexViewModel
            {
                Categories = itemsToDisplay,
                SearchTerm = searchTerm,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = totalItems
                }
            };

            return View(viewModel);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var category = await _unitOfWork.Categories.GetCategoryWithProductsByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var productViewModels = category.Products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                CategoryName = category.Name
            }).ToList();

            var viewModel = new CategoryDetailsViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                Products = productViewModels
            };

            return View(viewModel);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description
                };

                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.CompleteAsync();

                TempData["Success"] = "Category created successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryEditViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return View(viewModel);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var category = await _unitOfWork.Categories.GetByIdAsync(id);

                    if (category == null)
                    {
                        return NotFound();
                    }

                    category.Name = viewModel.Name;
                    category.Description = viewModel.Description;

                    await _unitOfWork.Categories.UpdateAsync(category);
                    await _unitOfWork.CompleteAsync();

                    TempData["Success"] = "Category updated successfully";
                }
                catch (Exception)
                {
                    TempData["Error"] = "An error occurred while updating the category";
                }

                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Categories.GetCategoryWithProductsByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.Products.Any())
            {
                TempData["Error"] = "Cannot delete category because it contains products";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = category.Products.Count
            };

            return View(viewModel);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _unitOfWork.Categories.GetCategoryWithProductsByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.Products.Any())
            {
                TempData["Error"] = "Cannot delete category because it contains products";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _unitOfWork.Categories.DeleteAsync(category);
                await _unitOfWork.CompleteAsync();
                TempData["Success"] = "Category deleted successfully";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the category";
            }

            return RedirectToAction(nameof(Index));
        }

        // Ajax action to check if category name exists
        [HttpGet]
        public async Task<IActionResult> CheckNameExists(string name, int? id)
        {
            var categories = await _unitOfWork.Categories.ListAllAsync();
            bool exists;

            if (id.HasValue)
            {
                // Edit mode - exclude current category from check
                exists = categories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Id != id.Value);
            }
            else
            {
                // Create mode
                exists = categories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

            return Json(!exists); // Return true if name is available (for jQuery validation)
        }

        // API endpoint to get product count by category for dashboard charts
        [HttpGet]
        public async Task<IActionResult> GetCategoryProductCounts()
        {
            var categories = await _unitOfWork.Categories.GetCategoriesWithProductsAsync();
            var data = categories.Select(c => new
            {
                category = c.Name,
                productCount = c.Products.Count
            });

            return Json(data);
        }
    }
}