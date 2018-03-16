using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}