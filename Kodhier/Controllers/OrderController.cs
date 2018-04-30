using System;
using System.Threading.Tasks;
using Kodhier.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Kodhier.Extensions;
using Kodhier.Models;
using Kodhier.Mvc;
using Kodhier.ViewModels.OrderViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Kodhier.Controllers
{
    public class OrderController : Controller
    {
        private readonly KodhierDbContext _context;

        public OrderController(KodhierDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var pizzas = _context.Pizzas
                    .Where(p => !p.IsDepricated)
                    .Select(p => new OrderViewModel
                    {
                        Name = p.Name,
                        Description = p.Description,
                        ImagePath = p.ImagePath,
                        PriceInfo = _context.PizzaPriceInfo
                        .Where(ppi => ppi.PriceCategoryId == p.PriceCategoryId)
                        .ToArray()
                    });
            return View(pizzas);
        }

        public async Task<IActionResult> Create(string id)
        {
            var exRes = new ExecutionResult();
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(Index));
            }

            var pizza = await _context.Pizzas
                .SingleOrDefaultAsync(m => m.Name == id);
            if (pizza == null)
            {
                exRes.AddError("Requested pizza could not be found.").PushTo(TempData);
                return RedirectToAction(nameof(Index));
            }

            var prices = _context.PizzaPriceInfo.Where(info => info.PriceCategoryId == pizza.PriceCategoryId);
            var vm = new OrderCreateViewModel
            {
                ImagePath = pizza.ImagePath,
                Name = pizza.Name,
                Prices = prices,
                Description = pizza.Description
            };
            vm.MinPrice = vm.Prices.DefaultIfEmpty(new PizzaPriceInfo()).Min(p => p.Price);
            return View(vm);
        }

        [Authorize]
        public IActionResult History()
        {
            return View(_context.Orders
                .Include(p => p.Pizza)
                .Where(o => o.Client.Id == User.GetId())
                .Where(o => o.IsPaid)
                .Select(o => new OrderHistoryViewModel
                {
                    Pizza = o.Pizza,
                    DateCreated = o.PlacementDate,
                    Price = o.Price,
                    Status = o.Status,
                    Size = o.Size,
                    Quantity = o.Quantity
                }));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string id, OrderCreateViewModel model)
        {
            var execRes = new ExecutionResult();

            var pizza = _context.Pizzas.SingleOrDefault(i => i.Name == id);
            if (pizza == null)
            {
                execRes.AddError("Requested pizza was not found. Please try again.").PushTo(TempData);
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.Prices = _context.PizzaPriceInfo.Where(info => info.PriceCategoryId == pizza.PriceCategoryId);
                model.MinPrice = model.Prices.Min(p => p.Price);
                return View(model);
            }

            if (pizza.IsDepricated)
            {
                execRes.AddError("Pizza no longer exists. Please try another pizza.").PushTo(TempData);
                return RedirectToAction(nameof(Index));
            }

            var ppi = _context.PizzaPriceInfo.SingleOrDefault(g => g.Id == model.SizeId);
            if (ppi == null || pizza.PriceCategoryId != ppi.PriceCategoryId)
            {
                execRes.AddError("Unexpected size was selected. Please try again.").PushTo(TempData);
                return RedirectToAction(nameof(Create), new { Id = id });
            }

            var userId = User.GetId();
            if (string.IsNullOrEmpty(userId))
            {
                execRes.AddError("You are logged out. Please log in to add the order.").PushTo(TempData);
                return RedirectToAction(nameof(Create), new { Id = id });
            }

            // Check if there is an order in the basket already
            var order = await _context.Orders.SingleOrDefaultAsync(o => !o.IsPaid && o.PizzaId == pizza.Id && o.ClientId == userId && o.Size == ppi.Size);
            if (order != null)
            {
                order.Price = ppi.Price;
                order.Quantity += model.Quantity;
                order.PlacementDate = DateTime.Now;
                // Adds a new line to the comment with the new comment.
                order.Comment = string.IsNullOrEmpty(order.Comment)
                    ? model.Comment : string.IsNullOrEmpty(model.Comment)
                        ? order.Comment : $"{order.Comment}\n-----\n{model.Comment}";
            }
            else
            {
                order = new Order
                {
                    Id = Guid.NewGuid(),
                    Pizza = pizza,
                    ClientId = userId,
                    Comment = model.Comment,
                    Quantity = model.Quantity,
                    Price = ppi.Price,
                    Size = ppi.Size,
                    PlacementDate = DateTime.Now
                };
                _context.Add(order);
            }

            if (await _context.SaveChangesAsync() > 0)
                execRes.AddInfo("Order was succesfully added to the basket.");
            else
                execRes.AddError("Order could not be added to the basket. Please try again.");

            execRes.PushTo(TempData);
            return RedirectToAction(nameof(Index));
        }
    }
}
