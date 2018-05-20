using System.Linq;
using Kodhier.Data;
using Kodhier.Extensions;
using Kodhier.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace Kodhier.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly KodhierDbContext _context;

        public NavbarViewComponent(KodhierDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var uid = HttpContext.User.GetId();
            var quantity = _context.Orders.Where(o => o.ClientId == uid).Count(o => !o.IsPaid);

            return View(new NavbarViewModel(quantity));
        }
    }
}
