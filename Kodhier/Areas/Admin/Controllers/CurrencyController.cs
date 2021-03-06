﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kodhier.Data;
using Kodhier.Models;
using Kodhier.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CurrencyController : Controller
    {
        private readonly KodhierDbContext _context;

        public CurrencyController(KodhierDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new PrepaidCardViewModel
            {
                Elements = _context.PrepaidCodes.Include(e => e.Redeemer)
                    .Select(c => new PrepaidCardViewModel
                    {
                        Id = c.Id,
                        Redeemer = c.Redeemer,
                        Amount = c.Amount,
                        CreationDate = c.CreationDate,
                        RedemptionDate = c.RedemptionDate
                    })
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Amount")] PrepaidCardViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            _context.PrepaidCodes.Add(new PrepaidCode
            {
                Id = Guid.NewGuid(),
                Amount = model.Amount,
                CreationDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
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
