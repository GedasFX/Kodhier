using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Extensions;
using Kodhier.Services;
using Kodhier.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Microsoft.Extensions.Caching.Memory;

namespace Kodhier.Controllers
{
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

        private IQueryable<CheckoutViewModel> GetCheckoutOrders(string clientId)
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
            return View(GetCheckoutOrders(User.GetId()));
        }

        [Authorize]
        public async Task<IActionResult> Edit(string id, int qty)
        {
            var clientId = User.GetId();
            var orderId = GetCheckoutOrders(clientId).Single(o => o.Id.ToString().Equals(id)).Id;

            var order = _context.Orders.Single(o => o.Id.Equals(orderId));

            order.Quantity = qty;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Continue()
        {
            var clientId = User.GetId();
            var user = _context.Users.Single(u => u.Id == clientId);

            var vm = new ConfirmCheckoutViewModel
            {
                CheckoutList = GetCheckoutOrders(clientId),
                ConfirmAddress = user.Address
            };

            vm.Price = vm.CheckoutList.Sum(o => o.Price * o.Quantity);

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmCheckoutViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Continue));

            var clientId = User.GetId();
            var orders = GetCheckoutOrders(clientId);

            var price = orders.Sum(o => o.Price * o.Quantity);
            var user = _context.Users.Single(u => u.Id == clientId);

            if (price > user.Coins)
            {
                // insufficient PizzaCoins
                return RedirectToAction("Index");
            }

            // Successful checkout, update db
            if (_context.Users.Single(u => u.Id == clientId).EmailSendUpdates)
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
                string messageBody = string.Format(builder.HtmlBody,
                    _context.Users.Single(u => u.Id == clientId).UserName
                    );

                await _emailSender.SendEmailAsync(_context.Users.Single(u => u.Id == clientId).Email, subject, messageBody);
            }

            user.Coins -= price;

            foreach (var checkoutEntry in orders)
            {
                var order = _context.Orders.Single(o => o.Id == checkoutEntry.Id);
                order.Status = Models.OrderStatus.Queued;
                order.IsPaid = true;
                order.DeliveryAddress = model.ConfirmAddress;
            }

            await _context.SaveChangesAsync();

            _cache.Remove(user.UserName);
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