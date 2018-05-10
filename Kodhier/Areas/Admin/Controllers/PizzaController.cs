using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Models;
using Kodhier.Mvc;
using Kodhier.ViewModels.Admin.PizzaViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PizzaController : Controller
    {
        private readonly KodhierDbContext _context;
        private readonly string _rootPath;

        public PizzaController(KodhierDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _rootPath = env.WebRootPath;
        }

        public IActionResult Index()
        {
            var vm = _context.Pizzas
                .Where(p => !p.IsDepricated)
                .Select(r => new PizzaViewModel
                {
                    Name = r.NameLt,
                    Description = r.DescriptionEn,
                    ImagePath = r.ImagePath,
                    PriceInfo = _context.PizzaPriceInfo
                        .Where(ppi => ppi.PriceCategoryId == r.PriceCategoryId)
                        .ToArray()
                });
            return View(vm);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizzas
                .SingleOrDefaultAsync(m => m.NameLt == id);
            if (pizza == null)
            {
                return NotFound();
            }

            var model = new PizzaDetailsViewModel
            {
                NameLt = pizza.NameLt,
                DescriptionLt = pizza.DescriptionLt,
                NameEn = pizza.NameEn,
                DescriptionEn = pizza.DescriptionEn,
                ImagePath = pizza.ImagePath,
                Prices = _context.PizzaPriceInfo.Where(ppi => ppi.PriceCategoryId == pizza.PriceCategoryId)
            };
            return View(model);
        }

        // GET: Pizza/Create
        public IActionResult Create()
        {
            var imgList = Directory.EnumerateFiles(Path.Combine(_rootPath, "uploads/img/gallery/"), "*.jpg")
                .Concat(Directory.EnumerateFiles(Path.Combine(_rootPath, "uploads/img/gallery/"), "*.png"))
                .Select(Path.GetFileName);

            return View(new PizzaCreateViewModel
            {
                PriceCategories = _context.PizzaPriceCategories,
                ImageList = imgList
            });
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
                NameLt = model.Name,
                PriceCategoryId = model.PriceCategoryId,
                Id = Guid.NewGuid(),
                DescriptionLt = model.Description,
                ImagePath = "~/uploads/img/gallery/" + model.ImagePath
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

            var pizza = await _context.Pizzas.SingleOrDefaultAsync(m => m.NameLt == id);
            if (pizza == null)
            {
                return NotFound();
            }

            var imgList = Directory.EnumerateFiles(Path.Combine(_rootPath, "uploads/img/gallery/"), "*.jpg")
                .Concat(Directory.EnumerateFiles(Path.Combine(_rootPath, "uploads/img/gallery/"), "*.png"))
                .Select(Path.GetFileName);

            var vm = new PizzaEditViewModel
            {
                Name = pizza.NameLt,
                PriceCategoryId = pizza.PriceCategoryId,
                Description = pizza.DescriptionLt,
                ImagePath = pizza.ImagePath,
                PriceCategories = _context.PizzaPriceCategories,
                ImageList = imgList
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

            var pizza = _context.Pizzas.Single(p => p.NameLt == id);
            pizza.NameLt = model.Name;
            pizza.DescriptionLt = model.Description;
            pizza.ImagePath = "~/uploads/img/gallery/" + model.ImagePath;
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
                .SingleOrDefaultAsync(m => m.NameLt == id);
            if (pizza == null)
            {
                return NotFound();
            }

            return View(new PizzaDeleteViewModel
            {
                Name = pizza.NameLt,
                Description = pizza.DescriptionLt,
                ImagePath = pizza.ImagePath,
                Prices = _context.PizzaPriceInfo.Where(ppi => ppi.PriceCategoryId == pizza.PriceCategoryId)
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var pizza = await _context.Pizzas.SingleOrDefaultAsync(m => m.NameLt == id);
            _context.Pizzas.Remove(pizza);

            var execRes = new ExecutionResult();
            try
            {
                if (await _context.SaveChangesAsync() > 0)
                    execRes.AddSuccess("Pizza deleted successfully.");
                else
                    execRes.AddError("Database error. Pizza was not deleted.");
            }
            catch (DbUpdateException)
            {
                _context.Entry(pizza).State = EntityState.Unchanged;
                pizza.IsDepricated = true;
                if (await _context.SaveChangesAsync() > 0)
                    execRes.AddInfo("Pizza could not be fully deleted. Changed to depricated.");
                else
                    execRes.AddError("Database error. Pizza was not deleted.");
            }

            execRes.PushTo(TempData);
            return RedirectToAction(nameof(Index));
        }
    }
}
