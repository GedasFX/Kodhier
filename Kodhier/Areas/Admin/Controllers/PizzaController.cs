using System;
using System.Linq;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Models;
using Kodhier.ViewModels.Admin.PizzaViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PizzaController : Controller
    {
        private readonly KodhierDbContext _context;

        public PizzaController(KodhierDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Pizzas.Include(c => c.PriceCategory)
                .Select(r => new PizzaViewModel
                {
                    Name = r.Name,
                    Description = r.Description,
                    PriceCategory = r.PriceCategory,
                    ImagePath = r.ImagePath,
                    MinPrice = _context.PizzaPriceInfo
                        .Where(ppi => ppi.PriceCategoryId == r.PriceCategory.Id)
                        .Min(c => c.Price)
                }));
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizzas.Include(p => p.PriceCategory)
                .SingleOrDefaultAsync(m => m.Name == id);
            if (pizza == null)
            {
                return NotFound();
            }

            var model = new PizzaDetailsViewModel
            {
                Name = pizza.Name,
                PriceCategory = pizza.PriceCategory,
                Description = pizza.Description,
                ImagePath = pizza.ImagePath
            }.EnumeratePrices(_context.PizzaPriceInfo);
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
        public async Task<IActionResult> Create([Bind("Name,PriceCategory,ImagePath,Description")] PizzaCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dbPizza = new Pizza
            {
                Name = model.Name,
                PriceCategory = model.PriceCategory,
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

            var pizza = await _context.Pizzas.Include(p => p.PriceCategory).SingleOrDefaultAsync(m => m.Name == id);
            if (pizza == null)
            {
                return NotFound();
            }

            var vm = new PizzaEditViewModel
            {
                Name = pizza.Name,
                PriceCategory = pizza.PriceCategory,
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
        public async Task<IActionResult> Edit(string id, [Bind("Name,PriceCategory,ImagePath,Description")] PizzaEditViewModel model)
        {
            if (id == string.Empty)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(model);

            var pizzaDb = _context.Pizzas.Single(p => p.Name == id);
            var pizza = new Pizza
            {
                Name = model.Name,
                PriceCategory = model.PriceCategory,
                Id = pizzaDb.Id,
                Description = model.Description,
                ImagePath = model.ImagePath
            };

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

            var pizza = await _context.Pizzas.Include(p => p.PriceCategory)
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
                Prices = _context.PizzaPriceInfo.Where(ppi => ppi.PriceCategoryId == pizza.PriceCategory.Id)
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
