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
                   .Where(o => o.Client.Id == HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value)
                   .Select(o => Mapper.Map<OrderViewModel>(o)));
        }

        public IActionResult Edit() // edit details like amount
        {
            return View();
        }

        public IActionResult Remove(Guid id)
        {
            var order = _context.Orders
                   .Where(o => o.Client.Id == HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value)
                   .Where(o => o.Id == id)
                   .Single();
            _context.Orders.Remove(order);

            return RedirectToAction("Index");
        }
    }
}