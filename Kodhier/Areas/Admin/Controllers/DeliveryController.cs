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

        // TODO: Talk about implementation
		public IActionResult Index()
		{
			var clientId = User.GetId();

			// check carefuly
			var orders = _context.Orders
				.Where(o => o.Status == OrderStatus.Delivering) // && o.DelivereeId == clientId
				.OrderByDescending(o => o.PaymentDate)
				//.Take(50) // limit due to google maps api
				.Select(o => new DeliveryViewModel
				{
					Id = o.Id.ToString(),
					Quantity = o.Quantity,
					Size = o.Size,
					Comment = o.Comment,
					Name = o.Pizza.NameLt,
					ImagePath = o.Pizza.ImagePath,
					DeliveryAddress = o.DeliveryAddress,
					DeliveryColor = ColorCode.Unassigned //o.DeliveryColor
				});
			return View(orders);
		}

		// Ready <-> Delivering -> Done
		// TODO: Assign each order a deliveree?
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Assign()
		{
			var newOrder = await _context.Orders.OrderBy(o => o.PaymentDate)
				.FirstOrDefaultAsync(o => o.Status == OrderStatus.Ready); // && o.DelivereeId == clientId
			if (newOrder == null)
				return RedirectToAction(nameof(Index));

			//newOrder.DeliveringDate = DateTime.Now();
			//newOrder.DelivereeId = User.GetId();
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

            var correctOrder = _context.Orders
				.Where(o => o.Id == id)
				.SingleOrDefault(o => o.Status == OrderStatus.Delivering);// && o.DelivereeId == clientId

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
				.Where(o => o.Id == id)
				.SingleOrDefault(o => o.Status == OrderStatus.Delivering);

			if (wrongOrder != null)
			{
				//wrongOrder.DeliveringDate = null;
				//wrongOrder.DelivereeId = null;
				wrongOrder.Status = OrderStatus.Ready;
			}

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> ChangeColor(Guid? id, String Color)
		{
			if (id == null)
				return RedirectToAction("Index");

			ColorCode colorCode;
			switch (Color)
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
			return RedirectToAction("Index/"+Color);
		}
	}
}