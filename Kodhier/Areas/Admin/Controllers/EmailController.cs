using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EmailController : Controller
    {
        [ActionName("Index")]
        public IActionResult Email()
        {
            return View(null);
        }
    }
}