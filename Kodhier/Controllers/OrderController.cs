using System;
using System.Threading.Tasks;
using Kodhier.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kodhier.Models;
using AutoMapper;
using Kodhier.Areas.Admin.ViewModels;
using Kodhier.ViewModels;
using System.Linq;
using System.Security.Claims;
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
            return View(_context.Pizzas.Select(p => Mapper.Map<PizzaViewModel>(p)));
        }

        [Authorize]
        public async Task<IActionResult> Create(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizzas
                .SingleOrDefaultAsync(m => m.Id == id);
            if (pizza == null)
            {
                return NotFound();
            }

            return View(new OrderCreateViewModel { Order = new OrderViewModel(), ImagePath = pizza.ImagePath, Name = pizza.Name, Price = pizza.Price });
        }

        [Authorize]
        public IActionResult History()
        {
            return View(_context.Orders
                .Where(o => o.Client.Id == HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value)
                .Where(o => o.IsPaid)
                .Select(o => Mapper.Map<OrderViewModel>(o)));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("Order,Name,Price,ImagePath")] OrderCreateViewModel model)
        {
            var pizza = _context.Pizzas.SingleOrDefault(i => i.Id == id);
            if (pizza == null)
                return View(model);
            if (ModelState.IsValid)
            {
                var order = Mapper.Map<Order>(model.Order);
                order.Id = Guid.NewGuid();
                order.Pizza = pizza;
                order.Client = _context.Users.SingleOrDefault(u => u.Id == HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
                _context.Add(order);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Your order was accepted successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
