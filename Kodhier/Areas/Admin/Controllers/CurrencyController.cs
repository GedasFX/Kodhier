using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Kodhier.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kodhier.Data;
using Kodhier.Models;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CurrencyController : Controller
    {
        private readonly KodhierDbContext _context;

        public CurrencyController(KodhierDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new PrepaidCardViewModel { Elements = _context.
                PrepaidCodes.Select(c => Mapper.Map<PrepaidCardViewModel>(c)) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Amount")] PrepaidCardViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.PrepaidCodes.Add(new PrepaidCode
                {
                    Id = Guid.NewGuid(),
                    Amount = model.Amount,
                    CreationDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var prepaidCode = await _context.PrepaidCodes.SingleOrDefaultAsync(m => m.Id == id);
            if (prepaidCode == null) return RedirectToAction(nameof(Index));
            _context.PrepaidCodes.Remove(prepaidCode);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
