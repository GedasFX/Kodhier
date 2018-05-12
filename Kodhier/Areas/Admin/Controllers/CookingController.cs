using System;
using System.Linq;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Extensions;
using Kodhier.Models;
using Kodhier.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Chef")]
    public class CookingController : Controller
    {
        private readonly KodhierDbContext _context;

        public CookingController(KodhierDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            var userid = User.GetId();
            var vm = new CookingViewModel
            {
                Queue = await _context.Orders.Include(o => o.Pizza)
                    .Where(o => o.ChefId == userid && o.Status == OrderStatus.Cooking)
                    .ToArrayAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign()
        {
            var order = await _context.Orders.OrderBy(o => o.PaymentDate)
                .FirstOrDefaultAsync(o => o.Status == OrderStatus.Queued && o.ChefId == null);
            if (order == null)
                return RedirectToAction(nameof(Index));

            order.ChefId = User.GetId();
            order.CookingDate = DateTime.Now;
            order.Status = OrderStatus.Cooking;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders.SingleOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                order.ReadyDate = DateTime.Now;
                order.Status = OrderStatus.Ready;
                await _context.SaveChangesAsync();
            }


            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Abandon(Guid? id)
        {
            if (id == null)
                return NotFound();

            var order = _context.Orders.Single(o => o.Id == id);

            order.ChefId = null;
            order.CookingDate = null;
            order.Status = OrderStatus.Queued;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public int AvailableCount()
        {
            return _context.Orders.Count(o => o.Status == OrderStatus.Queued && o.ChefId == null);
        }
    }
}