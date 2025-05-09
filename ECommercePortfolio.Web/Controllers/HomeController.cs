// ECommercePortfolio.Web/Controllers/HomeController.cs
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECommercePortfolio.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var latestProducts = await _unitOfWork.Products.ListAsync(p => p.IsActive);

            var productViewModels = latestProducts
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId
                })
                .ToList();

            return View(productViewModels);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}