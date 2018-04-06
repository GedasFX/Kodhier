using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kodhier.Data;
using Kodhier.Models;
using Kodhier.Areas.Admin.ViewModels;
using AutoMapper;

namespace Kodhier.Controllers.Admin
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
            return View(_context.Pizzas.Select(r => Mapper.Map<PizzaViewModel>(r)));
        }

        // GET: Pizza/Details/5
        public async Task<IActionResult> Details(Guid? id)
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
            return View(Mapper.Map<PizzaViewModel>(pizza));
        }

        // GET: Pizza/Create
        public IActionResult Create()
        {
            return View();
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
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizzas.SingleOrDefaultAsync(m => m.Id == id);
            if (pizza == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<PizzaViewModel>(pizza));
        }

        // POST: Pizza/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Price,Size,ImagePath,Description")] PizzaViewModel model)
        {
            if (id != model.Id)
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
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Pizza/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
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

            return View(Mapper.Map<PizzaViewModel>(pizza));
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
