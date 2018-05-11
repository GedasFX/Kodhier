using Microsoft.AspNetCore.Mvc;

namespace Kodhier.Areas.Admin.Controllers
{
    public class EmailController : Controller
    {
        [ActionName("Index")]
        public IActionResult Email()
        {
            return View();
        }
    }
}