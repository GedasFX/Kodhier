using System.Linq;
using System.Security.Claims;
using Kodhier.Data;
using Kodhier.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace Kodhier.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly KodhierDbContext _context;
        public NavbarViewComponent( KodhierDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            int quantity;

            var uid = User.GetId();

            quantity = _context.Orders.Where(o => o.Client.Id == uid).Where(o => !o.IsPaid).Count();
           

            return View(new NavbarViewModel(quantity));
        }
    }
}
