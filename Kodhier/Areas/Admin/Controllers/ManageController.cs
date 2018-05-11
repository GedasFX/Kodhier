using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Models;
using Kodhier.ViewModels.Admin.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kodhier.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ManageController : Controller
    {
        private readonly KodhierDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string[] _roles = { "Admin", "Delivery", "Chef" };

        public ManageController(
            KodhierDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ActionResult> Index(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            id = id.Normalize();
            var user = await _context.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == id);
            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            return View(new ManageDetailsViewModel
            {
                UserRoles = userRoles,
                Username = user.UserName,
                AllRoles = _roles
            });
        }

        [HttpPost]
        public async Task<ActionResult> Create(string id, ManageDetailsViewModel model)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            id = id.Normalize();
            var user = await _context.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == id);
            if (user == null)
                return NotFound();

            if (!await _roleManager.RoleExistsAsync(model.NewRole))
                await _roleManager.CreateAsync(new IdentityRole(model.NewRole));

            if (!await _userManager.IsInRoleAsync(user, model.NewRole))
                await _userManager.AddToRoleAsync(user, model.NewRole);

            return RedirectToAction(nameof(Index), "Manage", new { id = user.UserName });
        }

        // GET: Manage/Delete/5
        public async Task<ActionResult> Delete(string id, string role)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            id = id.Normalize();
            var user = await _context.Users.SingleOrDefaultAsync(u => u.NormalizedUserName == id);
            if (user == null)
                return NotFound();

            await _userManager.RemoveFromRoleAsync(user, role);

            return RedirectToAction(nameof(Index), "Manage", new { id = user.UserName });
        }
    }
}