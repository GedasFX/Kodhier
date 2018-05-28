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
    [Authorize(Roles = "Delivery")]
    public class DeliveryController : Controller
    {
        private readonly KodhierDbContext _context;

        public DeliveryController(KodhierDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.GetId();

            var orders = _context.Orders
                .Where(o => o.Status == OrderStatus.Delivering && o.DelivereeId == userId)
                .OrderByDescending(o => o.PaymentDate)
                .Select(o => new DeliveryViewModel
                {
                    Id = o.Id.ToString(),
                    Quantity = o.Quantity,
                    Size = o.Size,
                    Comment = o.DeliveryComment,
                    Name = o.Pizza.NameEn,
                    ImagePath = o.Pizza.ImagePath,
                    DeliveryAddress = o.DeliveryAddress,
                    DeliveryColor = o.DeliveryColor,
                    PhoneNumber = o.PhoneNumber
                });
            return View(orders);
        }

        // Ready <-> Delivering -> Done
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign()
        {
            var newOrder = await _context.Orders.OrderBy(o => o.PaymentDate)
                .FirstOrDefaultAsync(o => o.Status == OrderStatus.Ready && o.DelivereeId == null);
            if (newOrder == null)
                return RedirectToAction(nameof(Index));

            newOrder.DelivereeId = User.GetId();
            newOrder.DeliveryDate = DateTime.Now;
            newOrder.Status = OrderStatus.Delivering;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(Guid? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var correctOrder = await _context.Orders.SingleOrDefaultAsync(o => o.Id == id);

            if (correctOrder != null)
            {
                correctOrder.CompletionDate = DateTime.Now;
                correctOrder.Status = OrderStatus.Done;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Abandon(Guid? id)
        {
            if (id == null)
                return NotFound();

            var wrongOrder = _context.Orders
                .SingleOrDefault(o => o.Id == id);

            if (wrongOrder != null)
            {
                wrongOrder.Status = OrderStatus.Ready;
                wrongOrder.DeliveryDate = null;
                wrongOrder.DelivereeId = null;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeColor(Guid? id, string color)
        {
            if (id == null)
                return RedirectToAction("Index");

            ColorCode colorCode;
            switch (color)
            {
                case "Red": colorCode = ColorCode.Red; break;
                case "Orange": colorCode = ColorCode.Orange; break;
                case "Yellow": colorCode = ColorCode.Yellow; break;
                case "Green": colorCode = ColorCode.Green; break;
                case "Blue": colorCode = ColorCode.Blue; break;
                case "Purple": colorCode = ColorCode.Purple; break;
                default: return RedirectToAction("Index");
            }

            var whatOrder = _context.Orders.SingleOrDefault(o => o.Id == id);
            if (whatOrder != null)
                whatOrder.DeliveryColor = colorCode;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public int AvailableCount()
        {
            return _context.Orders.Count(o => o.Status == OrderStatus.Ready && o.DelivereeId == null);
        }
    }
}