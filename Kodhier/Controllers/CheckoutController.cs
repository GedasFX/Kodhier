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

		private IQueryable<ViewModels.CheckoutViewModel> GetCheckoutOrders(String clientId)
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
            return View(GetCheckoutOrders(clientId));
		}

        [Authorize]
        public async Task<IActionResult> Edit(string id, int qty)
        {
            var clientId = User.GetId();
			var orderID = GetCheckoutOrders(clientId).Single(o => o.Id.ToString().Equals(id)).Id;

			var order = _context.Orders.Single(o => o.Id.Equals(orderID));

			order.Quantity = qty;
			_context.Update(order);
			await _context.SaveChangesAsync();

            return RedirectToAction("Index");
		}

		[Authorize]
		public IActionResult Continue()
		{
			var vm = new ConfirmCheckoutViewModel();

			var clientId = User.GetId();
			var user = _context.Users.Where(u => u.Id == clientId).Single();
			vm.CheckoutList = GetCheckoutOrders(clientId);
			vm.ConfirmAddress = user.Address;
			vm.Price = vm.CheckoutList.Sum(o => o.Price * o.Quantity);

			//decimal wallet = _context.Users.Where(u => u.Id == clientId).Single().Coins;

			return View(vm);
		}

		[Authorize]
		public IActionResult Confirm(String confirmAddress)
		{
			var clientId = User.GetId();
			var orders = GetCheckoutOrders(clientId);

			decimal price = orders.Sum(o => o.Price * o.Quantity);
			var user = _context.Users.Where(u => u.Id == clientId).Single();

			if (price > user.Coins)
			{
				return RedirectToAction("Index");
				// insufficient pizzaCoins
			}

			// successful checkout, update db
			user.Coins -= price;
			_context.Users.Update(user);

			foreach (var checkoutEntry in orders)
			{
				var order = _context.Orders.Where(o => o.Id == checkoutEntry.Id).Single();
				order.Status = Models.OrderStatus.Queued;
				order.DeliveryAddress = user.Address;
				_context.Orders.Update(order);
			}

			//TempData["CheckoutSuccess"] = true;

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Remove(Guid id)
        {
			var order = _context.Orders
				   .Where(o => o.Client.Id == User.GetId())
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