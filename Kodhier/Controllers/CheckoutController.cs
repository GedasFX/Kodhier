using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Kodhier.Data;
using Kodhier.ViewModels;
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

        public IActionResult Index()
        {
            return View(_context.Orders
                   .Where(o => o.Client.Id == HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value));
        }

        public IActionResult Edit(Guid id) // edit details like amount
        {
            var order = _context.Orders
                   .Where(o => o.Client.Id == HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value)
                   .Where(o => o.Id == id)
                   .Single();

            if (order == null) // no such order
                return RedirectToAction("Index");

            return View(new OrderViewModel{ Pizza = order.Pizza, Size = order.Size, Quantity = order.Quantity });
        }

        public async Task<IActionResult> Edit(Guid id, OrderViewModel model)
        {
            var order = _context.Orders
                   .Where(o => o.Client.Id == HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value)
                   .Where(o => o.Id == id)
                   .Single();

            if (order == null) // no such order
                return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                order.Size = model.Size;
                order.Quantity = model.Quantity;

                _context.Update(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            //TempData["Success"] = "Incorrect data"; // display errors here
            return View(new OrderViewModel{ Pizza = order.Pizza, Size = order.Size, Quantity = order.Quantity });
        }

        public async Task<IActionResult> Remove(Guid id)
        {
            var order = _context.Orders
                   .Where(o => o.Client.Id == HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value)
                   .Where(o => o.Id == id)
                   .Single();

            if (order != null) {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}