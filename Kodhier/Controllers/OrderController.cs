using System;
using System.Linq;
using System.Threading.Tasks;
using Kodhier.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kodhier.Models;
using AutoMapper;
using Kodhier.Areas.Admin.ViewModels;
using Kodhier.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Kodhier.Controllers
{
    public class OrderController : Controller
    {
        private readonly KodhierDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(KodhierDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Pizzas.Select(p => Mapper.Map<PizzaViewModel>(p)));
        }

        // GET: Order/Details
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

            return View(new OrderCreateViewModel {Order = new OrderViewModel(), ImagePath = pizza.ImagePath, Name = pizza.Name, Price = pizza.Price });
        }

        // POST: Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("Order,Pizza")] OrderCreateViewModel model)
        {
            var pizza = _context.Pizzas.SingleOrDefault(i => i.Id == id);
            if (pizza == null)
                return View(model);
            if (ModelState.IsValid)
            {
                var order = Mapper.Map<Order>(model.Order);
                order.Id = Guid.NewGuid();
                order.Pizza = pizza;
                order.Client = _context.Users.SingleOrDefault(u => u.Id == _userManager.GetUserId(HttpContext.User).ToString());
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
