using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Extensions;
using Kodhier.Models;
using Kodhier.Mvc;
using Kodhier.Services;
using Kodhier.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace Kodhier.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly KodhierDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;
        private readonly IStringLocalizer<CheckoutController> _localizer;

        public CheckoutController(KodhierDbContext context,
            IEmailSender emailSender,
            IMemoryCache cache,
            IStringLocalizer<CheckoutController> localizer)
        {
            _context = context;
            _emailSender = emailSender;
            _cache = cache;
            _localizer = localizer;
        }

        private IQueryable<CheckoutViewModel> GetCheckoutOrders(string clientId, string culture)
        {
            var orders = _context.Orders
                .Where(o => o.Client.Id == clientId)
                .Where(o => o.Status == OrderStatus.Unpaid)
                .Where(o => o.PlacementDate > DateTime.Now.AddDays(-7))
                .OrderByDescending(c => c.PlacementDate)
                .Select(o => new CheckoutViewModel
                {
                    Id = o.Id,
                    Quantity = o.Quantity,
                    Size = o.Size,
                    Comment = o.CookingComment,
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
                execRes.AddError(_localizer["Pizza you were trying to edit was not found. Please try again."]).PushTo(TempData);
                return RedirectToAction("Index");
            }

            if (qty < 1)
            {
                execRes.AddError(_localizer["Quantity must be a positive whole number"]).PushTo(TempData);
                return RedirectToAction(nameof(Index));
            }

            var oq = order.Quantity;
            if (qty == oq)
            {
                return RedirectToAction(nameof(Index));
            }

            order.Quantity = qty;

            if (await _context.SaveChangesAsync() > 0)
                execRes.AddSuccess(_localizer["Pizza amount was successfully changed from {0} to {1}.", oq, qty]);
            else
            {
                execRes.AddError(_localizer["Order could not be processed. Please try again."]);
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
                new ExecutionResult().AddError(_localizer["Your email is not confirmed. Please confirm it before proceeding."]).PushTo(TempData);
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

            var orders = await _context.Orders.Include(o => o.Pizza)
                .Where(o => o.Client.Id == clientId)
                .Where(o => o.Status == OrderStatus.Unpaid)
                .Where(o => o.PlacementDate > DateTime.Now.AddDays(-7)) // Each item is only valid for 7 days
                .ToArrayAsync();

            var price = orders.Sum(o => o.Price * o.Quantity);

            if (price > user.Coins)
            {
                // insufficient PizzaCoins
                execRes.AddError(_localizer["Insufficient amount of coins in the balance."]).PushTo(TempData);
                return RedirectToAction("Index");
            }

            // looks good, go ahead

            foreach (var checkoutEntry in orders)
            {
                var order = _context.Orders.Single(o => o.Id == checkoutEntry.Id);
                order.Status = OrderStatus.Queued;
                order.IsPaid = true;
                order.DeliveryAddress = model.ConfirmAddress;
                order.DeliveryComment = model.Comment;
                order.PaymentDate = DateTime.Now;
            }

            user.Coins -= price;

            if (await _context.SaveChangesAsync() > 0)
                execRes.AddSuccess(_localizer["Pizza was ordered successfully."]);
            else
            {
                execRes.AddError(_localizer["Order could not be processed. Please try again."]).PushTo(TempData);
                return RedirectToAction("Index");
            }

            if (user.EmailSendUpdates)
            {
                await SendEmail(user, orders.ToArray());
                execRes.AddInfo(_localizer["Email was sent to {0}", user.Email]);
            }

            _cache.Remove(user.UserName);

            execRes.PushTo(TempData);
            return RedirectToAction("Index", "Order");
        }

        private async Task SendEmail(ApplicationUser user, Order[] cart)
        {
            string template, css;
            using (var sourceReader = System.IO.File.OpenText("Templates/EmailTemplate/Confirm_Order.html"))
                template = sourceReader.ReadToEnd();
            using (var sourceReader = System.IO.File.OpenText("Templates/EmailTemplate/Confirm_Order.css"))
                css = sourceReader.ReadToEnd();

            var messageBody = FormatBody(template, css, user, cart);

            await _emailSender.SendEmailAsync(user.Email, _localizer["Confirm Checkout"], messageBody);
        }

        private string FormatBody(string template, string css, ApplicationUser user, IReadOnlyList<Order> cart)
        {
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var cultCode = requestCulture.RequestCulture.UICulture.Name;
            var totalPrice = cart.Sum(o => o.Price * o.Quantity);
            var htmlCart = new StringBuilder();

            foreach (var order in cart)
            {
                var name = cultCode == "lt-LT" ? order.Pizza.NameLt : order.Pizza.NameEn;
                htmlCart.Append("<tr>" +
                                    $"<td class=\"alignleft\">{name}</td>" +
                                    $"<td class=\"aligncenter\">{order.Price / order.Quantity} €</td>" +
                                    $"<td class=\"aligncenter\">{order.Quantity}</td>" +
                                    $"<td class=\"alignright\">{order.Price} €</td>" +
                                "</tr>");
            }
            // Format rules:
            // {0} - Amount paid. Includes money.
            // {1} - _localizer["Thank you for using Kodhier services"]
            // {2} - user
            // {3} - order id
            // {4} - payment date
            // {5} - formatted cart with html
            // {6} - _localizer["Total"]
            // {7} - total money with symbol
            // {8} - CSS of ducument
            // {9} - _localizer["Username"]
            // {10} - _localizer["Order ID"]
            // {11} - _localizer["Payment date"]
            // {12} - _localizer["Name"]
            // {13} - _localizer["Unit price"]
            // {14} - _localizer["Quantity"]
            // {15} - _localizer["Subtotal"]
            return string.Format(template,
                string.Format(_localizer["{0} € Paid"], totalPrice),
                _localizer["Thank you for using Kodhier services"],
                user.UserName,
                cart[0].Id,
                cart[0].PaymentDate,
                htmlCart,
                _localizer["Total"],
                $"{totalPrice} €",
                css,
                _localizer["Username"],
                _localizer["Order ID"],
                _localizer["Payment date"],
                _localizer["Name"],
                _localizer["Unit price"],
                _localizer["Quantity"],
                _localizer["Subtotal"]
            );
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
                    execRes.AddSuccess(_localizer["Pizza was successfuly removed from the cart."]);

                else
                    execRes.AddError(_localizer["Pizza was not able to be removed from the cart. Please try again."]);
            }
            else
                execRes.AddError(_localizer["Requested pizza was not found. Please try again."]);

            execRes.PushTo(TempData);
            return RedirectToAction("Index");
        }
    }
}