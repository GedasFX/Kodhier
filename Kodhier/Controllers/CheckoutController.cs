﻿using System;
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

        [Authorize]
        public IActionResult Index()
        {
            var clientId = User.GetId();

            var orders = _context.Orders
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
                });

            return View(orders);
        }

        //public async Task<IActionResult> Edit(Guid id, OrderViewModel model)
        //{
        //    var order = _context.Orders
        //        .Where(o => o.Client.Id == HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value)
        //        .Where(o => o.Id == id)
        //        .Single();

        //    if (order == null) // no such order
        //        return RedirectToAction("Index");
        //    if (ModelState.IsValid)
        //    {
        //        order.Size = model.Size;
        //        order.Quantity = model.Quantity;

        //        _context.Update(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    //TempData["Failure"] = "Incorrect data"; // display errors here
        //    //return View(new OrderViewModel{ Pizza = order.Pizza, Size = order.Size, Quantity = order.Quantity });
        //    return RedirectToAction(nameof(Index));
        //}

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