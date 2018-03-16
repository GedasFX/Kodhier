using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Kodhier.Areas.Admin.ViewModels;
using Kodhier.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GalleryController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly KodhierDbContext _context;

        public GalleryController(KodhierDbContext dbContext, IHostingEnvironment env)
        {
            _context = dbContext;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_context.Pizzas.Select(p => Mapper.Map<PizzaViewModel>(p)));
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile imgfile)
        {
            if (imgfile.Length > 512000)
            {
                ModelState.AddModelError(string.Empty, "Max file size 512 KB");
                return View();
            }

            if (imgfile.ContentType != "text/jpeg")
            {
                ModelState.AddModelError(string.Empty, "Only images allowed");
                return View();
            }
            
            var filePath = Path.Combine(_env.WebRootPath, "uploads/img/gallery/", Path.GetFileName(imgfile.FileName));
            using (var stream = new FileStream(filePath, FileMode.Create)) // might be other errors?
            {
                await imgfile.CopyToAsync(stream);
            }
            
            return View();
        }
    }
}