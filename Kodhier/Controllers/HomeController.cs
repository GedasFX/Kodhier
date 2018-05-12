using Kodhier.Data;
using Kodhier.ViewModels;
using Kodhier.ViewModels.OrderViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;

namespace Kodhier.Controllers
{
    public class HomeController : Controller
    {
		private readonly KodhierDbContext _context;

		public HomeController(KodhierDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
			var cultCode = requestCulture.RequestCulture.UICulture.Name;
			
			var pizzas = _context.Pizzas
				.Where(p => !p.IsDepricated)
				.OrderByDescending(p => p.Id) // date added? pls
				.Take(4)
				.Select(p => new HomeViewModel
				{
					Name = cultCode == "lt-LT" ? p.NameLt : p.NameEn,
					Description = cultCode == "lt-LT" ? p.DescriptionLt : p.DescriptionEn,
					ImagePath = p.ImagePath,
					MinPrice = _context.PizzaPriceInfo
						.Where(ppi => ppi.PriceCategoryId == p.PriceCategoryId)
						.Min(c => c.Price)
				});

			return View(pizzas);
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en-GB", culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}