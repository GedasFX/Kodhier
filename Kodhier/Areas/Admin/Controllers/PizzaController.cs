using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Kodhier.Data;
using Kodhier.Models;
using Kodhier.ViewModels.PizzaViewModels;
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

        // GET: Pizza
        public IActionResult Index()
        {
            return View(_context.Pizzas.Include(c => c.PriceCategory)
                .Select(r => Mapper.Map<PizzaViewModel>(r)).ToArray()
                .Select(x => x.EnumeratePrices(_context.PizzaPriceInfo)));
        }

        // GET: Pizza/Details/5
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

            var modal = Mapper.Map<PizzaViewModel>(pizza);
            var m1 = modal.EnumeratePrices(_context.PizzaPriceInfo);
            return View(m1);
        }

        // GET: Pizza/Create
        public IActionResult Create()
        {
            return View(new PizzaViewModel { PriceCategories = _context.PizzaPriceCategories });
        }

        // POST: Pizza/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Size,ImagePath,Description")] PizzaViewModel pizza)
        {
            if (ModelState.IsValid)
            {
                var dbPizza = Mapper.Map<Pizza>(pizza);
                dbPizza.Id = Guid.NewGuid();

                _context.Add(dbPizza);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pizza);
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

            var vm = Mapper.Map<PizzaViewModel>(pizza).EnumeratePrices(_context.PizzaPriceInfo);
            vm.PriceCategories = _context.PizzaPriceCategories;

            return View(vm);
        }

        // POST: Pizza/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Price,Size,ImagePath,Description")] PizzaViewModel model)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var pizza = Mapper.Map<Pizza>(model);
                try
                {
                    _context.Update(pizza);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PizzaExists(pizza.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
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

            return View(Mapper.Map<PizzaViewModel>(pizza).EnumeratePrices(_context.PizzaPriceInfo));
        }

        // POST: Pizza/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var pizza = await _context.Pizzas.SingleOrDefaultAsync(m => m.Id == id);
            _context.Pizzas.Remove(pizza);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PizzaExists(Guid id)
        {
            return _context.Pizzas.Any(e => e.Id == id);
        }
    }
}
