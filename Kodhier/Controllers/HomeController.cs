using Kodhier.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using Kodhier.Data;
using Microsoft.EntityFrameworkCore;

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
            var vm = _context.News
                .Include(n => n.Pizza)
                .Include(n => n.Pizza.PriceCategory)
                .Include(n => n.Pizza.PriceCategory.PizzaPriceInfos)
                .Where(n => n.IsActive)
                .Select(n => new SliderViewModel
                {
                    Title = cultCode == "lt-LT" ? n.TitleLt : n.TitleEn,
                    Caption = cultCode == "lt-LT" ? n.CaptionLt : n.CaptionEn,
                    Price = n.Pizza.PriceCategory.PizzaPriceInfos.Min(ppi => ppi.Price),
                    PizzaImgPath = n.ImagePath
                });
            return View(vm);
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