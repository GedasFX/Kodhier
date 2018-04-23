using System;
using System.Threading.Tasks;
using Kodhier.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Kodhier.Extensions;
using Kodhier.Models;
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
            var pizzas = _context.Pizzas.Select(p => new OrderViewModel
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
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var pizza = await _context.Pizzas
                .SingleOrDefaultAsync(m => m.Name == id);
            if (pizza == null)
            {
                return NotFound();
            }

            var prices = _context.PizzaPriceInfo.Where(info => info.PriceCategoryId == pizza.PriceCategoryId);
            var vm = new OrderCreateViewModel
            {
                ImagePath = pizza.ImagePath,
                Name = pizza.Name,
                Prices = prices,
                Description = pizza.Description
            };
            vm.MinPrice = vm.Prices.Min(p => p.Price);
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
        public async Task<IActionResult> Create(string id, [Bind("Quantity,SizeId,Comment")] OrderCreateViewModel model)
        {
            // TempData["CreateSuccess"] - resulting value
            TempData["CreateSuccess"] = false;
            var pizza = _context.Pizzas.SingleOrDefault(i => i.Name == id);
            if (pizza == null)
            {
                ModelState.AddModelError("Error", "Pizza doesn't exist");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            var gar = _context.PizzaPriceInfo.SingleOrDefault(g => g.Id == model.SizeId);
            if (gar == null)
                return View(model);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                Pizza = pizza,
                ClientId = User.GetId(),
                Comment = model.Comment,
                Quantity = model.Quantity,
                Price = gar.Price,
                Size = gar.Size,
                PlacementDate = DateTime.Now,
                PizzaPriceCategoryId = pizza.PriceCategoryId
            };
            if (string.IsNullOrEmpty(order.ClientId))
                return View(model);

            _context.Add(order);
            await _context.SaveChangesAsync();
            TempData["CreateSuccess"] = true;
            return RedirectToAction(nameof(Index));
        }
    }
}
