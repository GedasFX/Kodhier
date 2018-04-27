using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            if (string.IsNullOrWhiteSpace(description))
            {
                return RedirectToAction(nameof(Index));
            }

            var ppc = new PizzaPriceCategory { Description = description };
            await _context.PizzaPriceCategories.AddAsync(ppc);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, int size, decimal price)
        {
            if (id == 0 || size == 0 || price == 0)
                return RedirectToAction(nameof(Index));

            var ppi = new PizzaPriceInfo
            {
                PriceCategoryId = id,
                Price = price,
                Size = size
            };
            _context.PizzaPriceInfo.Add(ppi);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int size, decimal price)
        {
            if (id == 0 || size == 0 || price == 0)
                return RedirectToAction(nameof(Index));

            var ppi = await _context.PizzaPriceInfo.SingleOrDefaultAsync(p => p.Id == id);

            ppi.Size = size;
            ppi.Price = price;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: PriceCategory/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var ppc = await _context.PizzaPriceCategories.SingleAsync(p => p.Id == id);
            _context.PizzaPriceCategories.Remove(ppc);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // TODO: Warn that there are pizzas still attached to this price category
            }
            

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            if (id == 0)
                return RedirectToAction(nameof(Index));

            var ppi = await _context.PizzaPriceInfo.SingleAsync(p => p.Id == id);

            _context.PizzaPriceInfo.Remove(ppi);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}