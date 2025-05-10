// ECommercePortfolio.Web/Controllers/OrdersController.cs
using ECommercePortfolio.Core.Entities;
using ECommercePortfolio.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommercePortfolio.Infrastructure.Data;

namespace ECommercePortfolio.Web.Controllers
{
    [Authorize] // Ensures that only logged-in users can access orders
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = new List<OrderViewModel>();

            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                // Admins and managers can see all orders
                orders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Select(o => new OrderViewModel
                    {
                        Id = o.Id,
                        OrderNumber = o.OrderNumber,
                        CustomerName = o.User.UserName,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status,
                        OrderDate = o.OrderDate
                    })
                    .ToListAsync();
            }
            else
            {
                // Regular users can only see their own orders
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                orders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .Select(o => new OrderViewModel
                    {
                        Id = o.Id,
                        OrderNumber = o.OrderNumber,
                        CustomerName = o.User.UserName,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status,
                        OrderDate = o.OrderDate
                    })
                    .ToListAsync();
            }

            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Check if the user is authorized to view this order
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (order.UserId != userId)
                {
                    return Forbid();
                }
            }

            var viewModel = new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerName = order.User.UserName,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: Orders/Track
        public IActionResult Track()
        {
            return View();
        }

        // POST: Orders/Track
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Track(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                TempData["ErrorMessage"] = "Please enter a valid order number.";
                return View();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found. Please check the order number and try again.";
                return View();
            }

            // Non-logged in users can track orders, but we'll redirect to the details page
            // if the user is logged in and owns the order or is an admin
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (User.IsInRole("Admin") || User.IsInRole("Manager") || order.UserId == userId)
                {
                    return RedirectToAction(nameof(Details), new { id = order.Id });
                }
            }

            var viewModel = new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate
            };

            return View("TrackResult", viewModel);
        }

        // GET: Orders/Cancel/5
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Check if the user is authorized to cancel this order
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (order.UserId != userId)
                {
                    return Forbid();
                }
            }

            // Can only cancel orders that are Pending or Processing
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
            {
                TempData["ErrorMessage"] = "This order cannot be cancelled.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate
            });
        }

        // POST: Orders/Cancel/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Check if the user is authorized to cancel this order
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (order.UserId != userId)
                {
                    return Forbid();
                }
            }

            // Can only cancel orders that are Pending or Processing
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
            {
                TempData["ErrorMessage"] = "This order cannot be cancelled.";
                return RedirectToAction(nameof(Details), new { id });
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order has been cancelled successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}