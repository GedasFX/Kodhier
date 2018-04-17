using System;
using System.Linq;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Models;
using Kodhier.ViewModels.Admin.PizzaViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PizzaController : Controller
    {
        private readonly KodhierDbContext _context;

        public PizzaController(KodhierDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var pizzas = _context.Pizzas;
            return View(pizzas
                .Select(r => new PizzaViewModel
                {
                    Name = r.Name,
                    Description = r.Description,
                    ImagePath = r.ImagePath,
                    MinPrice = _context.PizzaPriceInfo
                        .Where(ppi => ppi.PriceCategoryId == r.PriceCategoryId)
                        .Min(c => c.Price)
                }));
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

            var model = new PizzaDetailsViewModel
            {
                Name = pizza.Name,
                Description = pizza.Description,
                ImagePath = pizza.ImagePath,
                Prices = _context.PizzaPriceInfo.Where(ppi => ppi.PriceCategoryId == pizza.PriceCategoryId)
            };
            return View(model);
        }

        // GET: Pizza/Create
        public IActionResult Create()
        {
            return View(new PizzaCreateViewModel { PriceCategories = _context.PizzaPriceCategories });
        }

        // POST: Pizza/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,PriceCategoryId,ImagePath,Description")] PizzaCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dbPizza = new Pizza
            {
                Name = model.Name,
                PriceCategoryId = model.PriceCategoryId,
                Id = Guid.NewGuid(),
                Description = model.Description,
                ImagePath = model.ImagePath
            };

            _context.Add(dbPizza);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Pizza/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizzas.SingleOrDefaultAsync(m => m.Name == id);
            if (pizza == null)
            {
                return NotFound();
            }

            var vm = new PizzaEditViewModel
            {
                Name = pizza.Name,
                PriceCategoryId = pizza.PriceCategoryId ?? 0,
                Description = pizza.Description,
                ImagePath = pizza.ImagePath,
                PriceCategories = _context.PizzaPriceCategories
            };

            return View(vm);
        }

        // POST: Pizza/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,PriceCategoryId,ImagePath,Description")] PizzaEditViewModel model)
        {
            if (id == string.Empty)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(model);

            var pizza = _context.Pizzas.Single(p => p.Name == id);
            pizza.Name = model.Name;
            pizza.Description = model.Description;
            pizza.ImagePath = model.ImagePath;
            pizza.PriceCategoryId = model.PriceCategoryId;

            try
            {
                _context.Update(pizza);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Pizzas.Any(e => e.Id == pizza.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Pizza/Delete/5
        public async Task<IActionResult> Delete(string id)
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

            return View(new PizzaDeleteViewModel
            {
                Name = pizza.Name,
                Description = pizza.Description,
                ImagePath = pizza.ImagePath,
                Prices = _context.PizzaPriceInfo.Where(ppi => ppi.PriceCategoryId == pizza.PriceCategoryId)
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var pizza = await _context.Pizzas.SingleOrDefaultAsync(m => m.Name == id);
            _context.Pizzas.Remove(pizza);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
