using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Models;
using Kodhier.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PriceCategoryController : Controller
    {
        private readonly KodhierDbContext _context;

        public PriceCategoryController(KodhierDbContext context)
        {
            _context = context;
        }

        // GET: PriceCategory
        public async Task<ActionResult> Index()
        {
            var ppi = await _context.PizzaPriceCategories.Include(ppc => ppc.PizzaPriceInfos).ToArrayAsync();
            return View(ppi);
        }

        // POST: PriceCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string description)
        {
            var execRes = new ExecutionResult();
            if (string.IsNullOrWhiteSpace(description))
            {
                execRes.AddError("Invalid description was given. Please try again.").PushTo(TempData);
                return RedirectToAction(nameof(Index));
            }

            var ppc = new PizzaPriceCategory { Description = description };
            await _context.PizzaPriceCategories.AddAsync(ppc);

            if (await _context.SaveChangesAsync() > 0)
            {
                execRes.AddSuccess("Price category was successfully added.");
            }
            else
            {
                execRes.AddError("Database error occured. Price category could not be added.");
            }

            execRes.PushTo(TempData);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, int size, decimal price)
        {
            var execRes = new ExecutionResult();
            if (id == 0 || size == 0 || price == 0)
            {
                execRes.AddError("Invalid size or price was given. Please try again.").PushTo(TempData);
                return RedirectToAction(nameof(Index));
            }

            var ppi = new PizzaPriceInfo
            {
                PriceCategoryId = id,
                Price = price,
                Size = size
            };

            await _context.PizzaPriceInfo.AddAsync(ppi);
            if (await _context.SaveChangesAsync() > 0)
            {
                execRes.AddSuccess("Price information was successfully added.");
            }
            else
            {
                execRes.AddError("Database error occured. Price information could not be added.");
            }
            
            execRes.PushTo(TempData);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int size, decimal price)
        {
            var execRes = new ExecutionResult();
            if (id == 0 || size == 0 || price == 0)
            {
                execRes.AddError("Invalid size or price was given. Please try again.").PushTo(TempData);
                return RedirectToAction(nameof(Index));
            }

            var ppi = await _context.PizzaPriceInfo.SingleOrDefaultAsync(p => p.Id == id);

            ppi.Size = size;
            ppi.Price = price;

            _context.PizzaPriceInfo.Update(ppi);
            if (await _context.SaveChangesAsync() > 0)
            {
                execRes.AddSuccess("Price information was successfully updated.");
            }
            else
            {
                execRes.AddError("Database error occured. Price information could not be added.");
            }

            execRes.PushTo(TempData);
            return RedirectToAction(nameof(Index));
        }

        // POST: PriceCategory/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int? id)
        {
            var execRes = new ExecutionResult();
            if (id == null)
            {
                execRes.AddError("Invalid id was provided. Please try again.").PushTo(TempData);
                return RedirectToAction(nameof(Index));
            }

            var ppc = await _context.PizzaPriceCategories.SingleAsync(p => p.Id == id);
            _context.PizzaPriceCategories.Remove(ppc);
            try
            {
                if (await _context.SaveChangesAsync() > 0)
                {
                    execRes.AddSuccess("Price category was sucessfully removed.");
                }
                else
                {
                    execRes.AddError("Invalid request.");
                }
            }
            catch (DbUpdateException)
            {
                execRes.AddError("Price category is still in use by some pizzas. Make sure to unlink all pizzas before proceeding.");
            }
            
            execRes.PushTo(TempData);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int? id)
        {
            var execRes = new ExecutionResult();
            if (id == null)
            {
                execRes.AddError("Invalid id was provided. Please try again.").PushTo(TempData);
                return RedirectToAction(nameof(Index));
            }

            var ppi = await _context.PizzaPriceInfo.SingleAsync(p => p.Id == id);

            _context.PizzaPriceInfo.Remove(ppi);

            if (await _context.SaveChangesAsync() > 0)
            {
                execRes.AddSuccess("Price information was sucessfully removed.");
            }
            else
            {
                execRes.AddError("Invalid request.");
            }

            execRes.PushTo(TempData);
            return RedirectToAction(nameof(Index));
        }
    }
}