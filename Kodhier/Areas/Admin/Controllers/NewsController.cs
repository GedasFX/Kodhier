using System.Linq;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Models;
using Kodhier.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        private readonly KodhierDbContext _context;

        public NewsController(KodhierDbContext context)
        {
            _context = context;
        }

        public async Task<ViewResult> Index()
        {
            while (_context.News.Count() < 4)
            {
                _context.News.Add(new News());
                await _context.SaveChangesAsync();
            }
            return View(new NewsViewModel
            {
                Slides = await _context.News.OrderBy(o => o.Id).Take(4).ToArrayAsync(),
                Pizzas = await _context.Pizzas.ToArrayAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, NewsViewModel model)
        {
            var news = await _context.News.SingleOrDefaultAsync(n => id == n.Id);

            news.Caption = model.Caption;
            news.IsActive = model.IsActive == "on";
            news.Title = model.Title;
            news.PizzaId = model.PizzaId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}