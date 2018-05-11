using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Extensions;
using Kodhier.Models;
using Kodhier.Mvc;
using Kodhier.Services;
using Kodhier.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Microsoft.Extensions.Caching.Memory;

namespace Kodhier.Controllers
{
	[Authorize]
	public class CheckoutController : Controller
	{
		private readonly KodhierDbContext _context;
		private readonly IEmailSender _emailSender;
		private readonly IHostingEnvironment _env;
		private readonly IMemoryCache _cache;

		public CheckoutController(KodhierDbContext context,
			IEmailSender emailSender,
			IHostingEnvironment env,
			IMemoryCache cache)
		{
			_context = context;
			_emailSender = emailSender;
			_env = env;
			_cache = cache;
		}

		private IQueryable<CheckoutViewModel> GetCheckoutOrders(string clientId, string culture)
		{
			var orders = _context.Orders
				.Where(o => o.Client.Id == clientId)
				.Where(o => o.Status == OrderStatus.Unpaid)
                .Where(o => o.PlacementDate > DateTime.Now.AddDays(-14))
				.OrderByDescending(c => c.PlacementDate)
				.Select(o => new CheckoutViewModel
				{
					Id = o.Id,
					Quantity = o.Quantity,
					Size = o.Size,
					Comment = o.Comment,
					Name = culture == "lt-LT" ? o.Pizza.NameLt : o.Pizza.NameEn,
					ImagePath = o.Pizza.ImagePath,
					Price = o.Price,
					Description = culture == "lt-LT" ? o.Pizza.DescriptionLt : o.Pizza.DescriptionEn
                });
			return orders;
		}

		public async Task<IActionResult> Index()
		{
		    var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
		    var cultCode = requestCulture.RequestCulture.UICulture.Name;
            var orders = await GetCheckoutOrders(User.GetId(), cultCode).ToListAsync();
			return orders.Count == 0 ? View("Empty") : View(orders);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, int qty)
		{
			var execRes = new ExecutionResult();
			var order = _context.Orders.Single(o => o.Id.Equals(Guid.Parse(id)));
			if (order == null || order.ClientId != User.GetId())
			{
				execRes.AddError("Pizza you were trying to edit was not found. Please try again.").PushTo(TempData);
				return RedirectToAction("Index");
			}

			var oq = order.Quantity;
			order.Quantity = qty;

			if (await _context.SaveChangesAsync() > 0)
				execRes.AddSuccess($"Pizza amount was successfully changed from {oq} to {qty}.");
			else
			{
				execRes.AddError("Order could not be processed. Please try again.");
			}

			execRes.PushTo(TempData);
			return RedirectToAction("Index");
		}

		public IActionResult Continue()
		{
			var clientId = User.GetId();
			var user = _context.Users.Single(u => u.Id == clientId);

			if (!user.EmailConfirmed)
			{
				new ExecutionResult().AddError("Your email is not confirmed. Please confirm it before proceeding.").PushTo(TempData);
				return RedirectToAction(nameof(Index), "Manage");
			}

		    var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
		    var cultCode = requestCulture.RequestCulture.UICulture.Name;
            var vm = new ConfirmCheckoutViewModel
			{
				CheckoutList = GetCheckoutOrders(clientId, cultCode),
				ConfirmAddress = user.Address,
				PhoneNumber = user.PhoneNumber
			};

			vm.Price = vm.CheckoutList.Sum(o => o.Price * o.Quantity);

			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Continue(ConfirmCheckoutViewModel model)
		{
			var execRes = new ExecutionResult();

			var clientId = User.GetId();
			var user = _context.Users.Single(u => u.Id == clientId);
		    
            if (!ModelState.IsValid)
			{
			    var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
			    var cultCode = requestCulture.RequestCulture.UICulture.Name;
                model.CheckoutList = GetCheckoutOrders(clientId, cultCode);
				model.Price = model.CheckoutList.Sum(o => o.Price * o.Quantity);

				return View(model);
			}

		    var orders = _context.Orders.Include(o => o.Pizza)
		        .Where(o => o.Client.Id == clientId)
		        .Where(o => o.Status == OrderStatus.Unpaid)
		        .Where(o => o.PlacementDate > DateTime.Now.AddDays(-14));
            
		    var price = orders.Sum(o => o.Price * o.Quantity);

			if (price > user.Coins)
			{
				// insufficient PizzaCoins
				execRes.AddError("Insufficient amount of coins in the balance.").PushTo(TempData);
				return RedirectToAction("Index");
			}

			// looks good, go ahead

			foreach (var checkoutEntry in orders)
			{
				var order = _context.Orders.Single(o => o.Id == checkoutEntry.Id);
				order.Status = OrderStatus.Queued;
				order.IsPaid = true;
				order.DeliveryAddress = model.ConfirmAddress;
			}

			user.Coins -= price;

			if (await _context.SaveChangesAsync() > 0)
				execRes.AddSuccess("Pizza was ordered successfully .");
			else
			{
				execRes.AddError("Order could not be processed. Please try again.").PushTo(TempData);
				return RedirectToAction("Index");
			}

			if (user.EmailSendUpdates)
			{
				await SendEmail(user);
				execRes.AddInfo($"Email was sent to {user.Email}");
			}

			_cache.Remove(user.UserName);

			execRes.PushTo(TempData);
			return RedirectToAction("Index", "Order");
		}

		private async Task SendEmail(ApplicationUser user)
		{
			var pathToFile = _env.WebRootPath
							 + Path.DirectorySeparatorChar
							 + "Templates"
							 + Path.DirectorySeparatorChar
							 + "EmailTemplate"
							 + Path.DirectorySeparatorChar
							 + "Confirm_Order.html";

			const string subject = "Confirm Checkout";
			var builder = new BodyBuilder();

			using (var sourceReader = System.IO.File.OpenText(pathToFile))
			{
				builder.HtmlBody = sourceReader.ReadToEnd();
			}

			var messageBody = string.Format(builder.HtmlBody, user.UserName);

			await _emailSender.SendEmailAsync(user.Email, subject, messageBody);
		}

		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Remove(Guid id)
		{
			var execRes = new ExecutionResult();

			var order = _context.Orders
				   .Where(o => o.Client.Id == User.GetId())
				   .Single(o => o.Id == id);

			if (order != null)
			{
				_context.Orders.Remove(order);
				if (await _context.SaveChangesAsync() > 0)
					execRes.AddSuccess("Pizza was successfuly removed from the basket.");

				else
					execRes.AddError("Pizza was not able to be removed from the basket. Please try again.");
			}
			else
				execRes.AddError("Requested pizza was not found. Please try again.");

			execRes.PushTo(TempData);
			return RedirectToAction("Index");
		}
	}
}