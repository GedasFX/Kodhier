using System;
using System.Linq;
using Kodhier.Data;
using Kodhier.Extensions;
using Kodhier.Models;
using Kodhier.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kodhier.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
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

			// check carefully
			var orders = _context.Orders
				.Where(o => o.Client.Id == clientId)
				.Where(o => o.Status == OrderStatus.Delivering)
				.OrderByDescending(c => c.PlacementDate)
				.Select(o => new DeliveryViewModel
				{
					Id = o.Id.ToString(),
					Quantity = o.Quantity,
					Size = o.Size,
					Comment = o.Comment,
					Name = o.Pizza.Name,
					ImagePath = o.Pizza.ImagePath,
					DeliveryAddress = o.DeliveryAddress
				});
			return View(orders);
		}

		public IActionResult Complete(Guid? id)
		{
            if (id == null)
                return RedirectToAction("Index");

            var correctOrder = _context.Orders
				.Where(o => o.Id == id)
				.SingleOrDefault(o => o.Status == OrderStatus.Delivering);

			if (correctOrder != null)
			{
				correctOrder.Status = OrderStatus.Done;
				_context.SaveChanges();
			}

			return RedirectToAction("Index");
		}
	}
}