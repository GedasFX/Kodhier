using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Chef,Delivery")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}