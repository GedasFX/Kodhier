using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Models;
using Kodhier.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        private readonly KodhierDbContext _context;
        private readonly IHostingEnvironment _env;

        public NewsController(KodhierDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<ViewResult> Index()
        {
            while (_context.News.Count() < 4)
            {
                _context.News.Add(new News());
                await _context.SaveChangesAsync();
            }

            var imgList = Directory.EnumerateFiles(Path.Combine(_env.WebRootPath, "uploads/img/gallery/"), "*.jpg")
                .Concat(Directory.EnumerateFiles(Path.Combine(_env.WebRootPath, "uploads/img/gallery/"), "*.png"))
                .Select(Path.GetFileName);

            return View(new NewsViewModel
            {
                Slides = await _context.News.OrderBy(o => o.Id).Take(4).ToArrayAsync(),
                Pizzas = await _context.Pizzas.ToArrayAsync(),
                Images = imgList
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, NewsViewModel model)
        {
            var news = await _context.News.SingleOrDefaultAsync(n => id == n.Id);

            if (model.PizzaId == null)
                return RedirectToAction(nameof(Index));

            news.CaptionLt = model.CaptionLt;
            news.CaptionEn = model.CaptionEn;
            news.IsActive = model.IsActive == "on";
            news.TitleLt = model.TitleLt;
            news.TitleEn = model.TitleEn;
            news.PizzaId = model.PizzaId;
            news.ImagePath = model.ImagePath;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}