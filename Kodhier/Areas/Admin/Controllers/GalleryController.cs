using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GalleryController : Controller
    {
        private readonly string _rootPath;

        public GalleryController(IHostingEnvironment env)
        {
            _rootPath = env.WebRootPath;
        }

        public IActionResult Index()
        {
            //var imgList = Directory.EnumerateFiles(Path.Combine(_rootPath, "uploads/img/gallery/"), "*.jpg").Select(item => "~/uploads/img/gallery/" + Path.GetFileName(item));
            var imgList = Directory.EnumerateFiles(Path.Combine(_rootPath, "uploads/img/gallery/"), "*.jpg")
                .Concat(Directory.EnumerateFiles(Path.Combine(_rootPath, "uploads/img/gallery/"), "*.png"))
                .Select(item => "~/uploads/img/gallery/" + Path.GetFileName(item));
            return View(imgList);
        }

        public async Task<IActionResult> Upload(IFormFile imgfile)
        {
            if (imgfile.Length > 512000)
            {
                ModelState.AddModelError(string.Empty, "Max file size 512 KB");
                return RedirectToAction("Index");
            }

            if (!(imgfile.ContentType == "image/jpeg" || imgfile.ContentType == "image/png"))
            {
                ModelState.AddModelError(string.Empty, "Only images allowed");
                return RedirectToAction("Index");
            }

            var filePath = Path.Combine(_rootPath, "uploads/img/gallery/", Path.GetFileName(imgfile.FileName));
            using (var stream = new FileStream(filePath, FileMode.Create)) // might be other errors?
            {
                await imgfile.CopyToAsync(stream);
            }

            return RedirectToAction("Index");
        }
    }
}