using System;
using System.Threading.Tasks;
using Kodhier.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kodhier.Models;
using AutoMapper;
using System.Linq;
using System.Security.Claims;
using Kodhier.ViewModels.OrderViewModels;
using Kodhier.ViewModels.PizzaViewModels;
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
            var pizzas = _context.Pizzas.Include(p => p.PriceCategory).Select(p => Mapper.Map<PizzaViewModel>(p));
            foreach (var pizza in pizzas)
            {
                pizza.Prices = _context.PizzaPriceInfo.Where(info => info.PriceCategoryId == pizza.PriceCategory.Id);
            }
            return View(pizzas);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizzas
                .SingleOrDefaultAsync(m => m.Name == id);
            if (pizza == null)
            {
                return NotFound();
            }

            return View(new OrderCreateViewModel { ImagePath = pizza.ImagePath, Name = pizza.Name, Prices = _context.PizzaPriceInfo.Where(info => info.PriceCategoryId == pizza.PriceCategory.Id), Description = pizza.Description });
        }

        [Authorize]
        public IActionResult History() =>
            View(_context.Orders
                .Include(p => p.Pizza)
                .Where(o => o.Client.Id ==
                            HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value)
                .Where(o => o.IsPaid)
                .Select(o => Mapper.Map<OrderViewModel>(o)));

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string id, [Bind("Order,Name,Price,ImagePath")] OrderCreateViewModel model)
        {
            var pizza = _context.Pizzas.SingleOrDefault(i => i.Name == id);
            if (pizza == null)
            {
                ModelState.AddModelError("Error", "Invalid pizza size submited");
                return View(nameof(Details), model);
            }

            if (!ModelState.IsValid)
                return View(nameof(Details), model);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                Pizza = pizza,
                Client = _context.Users.SingleOrDefault(u =>
                    u.Id == HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value),
                Comment = model.Comment,
                Quantity = model.Quantity
            };
            _context.Add(order);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Your order was accepted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
