// ECommercePortfolio.Web/Areas/Admin/Controllers/DashboardController.cs
using ECommercePortfolio.Core.Interfaces;
using ECommercePortfolio.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ECommercePortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            // Get data for the dashboard
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var dashboard = new DashboardViewModel
            {
                TotalProducts = await _unitOfWork.Products.GetTotalProductCountAsync(),
                TotalInventoryValue = await _unitOfWork.Products.GetTotalInventoryValueAsync(),
                MonthlyOrders = await _unitOfWork.Orders.GetTotalOrdersAsync(startOfMonth, endOfMonth),
                MonthlySales = await _unitOfWork.Orders.GetTotalSalesAsync(startOfMonth, endOfMonth)
            };

            return View(dashboard);
        }
    }
}