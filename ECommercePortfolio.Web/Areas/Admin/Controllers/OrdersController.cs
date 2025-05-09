// ECommercePortfolio.Web/Areas/Admin/Controllers/OrdersController.cs
using ECommercePortfolio.Core.Entities;
using ECommercePortfolio.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ECommercePortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Manager")]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _unitOfWork.Orders.ListAllAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            await _unitOfWork.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}