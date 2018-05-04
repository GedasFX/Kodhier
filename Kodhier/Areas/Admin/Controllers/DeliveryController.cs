using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Extensions;
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

		public IActionResult Index()
		{
			var clientId = User.GetId();

			// check carefully
			var orders = _context.Orders
				.Where(o => o.Client.Id == clientId)
				.Where(o => o.Status == Models.OrderStatus.Delivering)
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

		public IActionResult Complete(String id)
		{
			var correctOrder = _context.Orders
				.Where(o => o.Id.ToString() == id)
				.Where(o => o.Status == Models.OrderStatus.Delivering)
				.SingleOrDefault();

			if (correctOrder != null)
			{
				correctOrder.Status = Models.OrderStatus.Done;
				_context.Update(correctOrder);
				_context.SaveChanges();
			}

			return RedirectToAction("Index");
		}
	}
}