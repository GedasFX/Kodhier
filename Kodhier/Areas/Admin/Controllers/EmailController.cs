using System.Linq;
using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Models;
using Kodhier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EmailController : Controller
    {
        private readonly KodhierDbContext _context;
        private readonly IEmailSender _emailSender;

        public EmailController(KodhierDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [ActionName("Index")]
        public IActionResult Email()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(string subject, string htmlMessage)
        {
            await SendAll(subject, htmlMessage);
            return RedirectToAction("Index");
        }

        private async Task SendAll(string subject, string message)
        {
            var users = _context.Users
                .Where(u => u.EmailConfirmed)
                .Where(u => u.EmailSendPromotional);
            foreach (var user in users)
            {
                await SendEmail(user, subject, message);
            }
        }

        private async Task SendEmail(ApplicationUser user, string subject, string message)
        {
            var messageBody = string.Format(message, user.UserName);

            await _emailSender.SendEmailAsync(user.Email, subject, messageBody);
        }
    }
}