using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Extensions;
using Kodhier.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kodhier.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly KodhierDbContext _context;

        public CheckoutController(KodhierDbContext context)
        {
            _context = context;
        }

		public IQueryable<ViewModels.CheckoutViewModel> getCheckoutOrders(String clientId)
		{
			return _context.Orders
				.Where(o => o.Client.Id == clientId)
				.Where(o => !o.IsPaid)
				.OrderByDescending(c => c.PlacementDate)
				.Select(o => new CheckoutViewModel
				{
					Id = o.Id,
					Quantity = o.Quantity,
					Size = o.Size,
					Comment = o.Comment,
					Name = o.Pizza.Name,
					ImagePath = o.Pizza.ImagePath,
					Price = o.Price,
					Description = o.Pizza.Description
				}
			);
		}

        [Authorize]
        public IActionResult Index()
        {
            var clientId = User.GetId();
            return View(getCheckoutOrders(clientId));
		}

        [Authorize]
        public async Task<IActionResult> Edit(string id, int qty)
        {
            var clientId = User.GetId();
			var orderID = getCheckoutOrders(clientId).Single(o => o.Id.ToString().Equals(id)).Id;

			var order = _context.Orders.Single(o => o.Id.Equals(orderID));

			order.Quantity = qty;
			_context.Update(order);
			await _context.SaveChangesAsync();

            return RedirectToAction("Index");
		}

		[Authorize]
		public IActionResult Continue()
		{
			var clientId = User.GetId();
			var orders = getCheckoutOrders(clientId);

			decimal price = orders.Sum(o => o.Price * o.Quantity);
			decimal wallet = _context.Users.Where(u => u.Id == clientId).Single().Coins;

			return View(price);
		}

		[Authorize]
		public IActionResult Confirm()
		{
			var clientId = User.GetId();
			var orders = getCheckoutOrders(clientId);

			decimal price = orders.Sum(o => o.Price * o.Quantity);
			decimal wallet = _context.Users.Where(u => u.Id == clientId).Single().Coins;

			if (price > wallet)
			{
				return RedirectToAction("Index");
				// insufficient pizzaCoins
			}


			// successful checkout, update db

			//TempData["CheckoutSuccess"] = true;

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Remove(Guid id)
        {
            var order = _context.Orders
                   .Where(o => o.Client.Id
                               == HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value)
                   .Single(o => o.Id == id);

            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}