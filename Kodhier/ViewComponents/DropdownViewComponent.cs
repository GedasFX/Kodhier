using System.Linq;
using System.Security.Claims;
using Kodhier.Data;
using Kodhier.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Kodhier.ViewComponents
{
    public class DropdownViewComponent : ViewComponent
    {
        private readonly IMemoryCache _cache;
        private readonly KodhierDbContext _context;
        public DropdownViewComponent(IMemoryCache cache, KodhierDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var name = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Name).Value;

            if (_cache.TryGetValue(name, out decimal amount))
                return View(new DropdownViewModel(name, amount));

            amount = _context.Users.Single(o => o.UserName == name).Coins;
            _cache.Set(name, amount);

            return View(new DropdownViewModel(name, amount));
        }
    }
}
