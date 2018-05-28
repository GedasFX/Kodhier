using Kodhier.Data;
using Kodhier.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Kodhier.Areas.Admin.Controllers;
using System.ComponentModel.DataAnnotations;
using Kodhier.Controllers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Kodhier.ViewModels;
using Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace Kodhier.Tests
{
    public class PrepaidCardCodeTests
    {
        readonly KodhierDbContext _context;
        readonly CurrencyController _controller;

        // Currency controller responsible for: creation, deletion
        public PrepaidCardCodeTests()
        {
            _context = new KodhierDbContext(new DbContextOptionsBuilder<KodhierDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _controller = new CurrencyController(_context);
        }

        [Fact(DisplayName = "Currency create code success")]
        public async Task Currency_Create_Code_Ok()
        {
            var beforeCount = _context.PrepaidCodes.Count();
            var amount = 1;
            var model = new PrepaidCardViewModel { Amount = amount };

            var result = await _controller.Create(model);


            var last = _context.PrepaidCodes.OrderBy(c => c.CreationDate).Last();
            Assert.Equal(amount, last.Amount);
            Assert.Equal(beforeCount + 1, _context.PrepaidCodes.Count());

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact(DisplayName = "Currency create code invalid amount")]
        public async Task Currency_Create_Code_Fail1()
        {
            var beforeCount = _context.PrepaidCodes.Count();
            var amount = -2;
            var model = new PrepaidCardViewModel { Amount = amount };

            if (!Validator.TryValidateObject(model, new ValidationContext(model), null, true))
                _controller.ModelState.AddModelError("err", "Error");

            var result = await _controller.Create(model);

            Assert.Equal(beforeCount, _context.PrepaidCodes.Count());

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact(DisplayName = "Currency delete code success")]
        public async Task Currency_Delete_Code_Ok() // delete any code
        {
            var cId = new Guid("ec047e08-4444-41d0-a208-15bd2dcb3389");
            var code1 = new PrepaidCode { Id = cId, Amount = 1, CreationDate = DateTime.Now };
            _context.PrepaidCodes.Add(code1);

            await _context.SaveChangesAsync();
            var beforeCount = _context.PrepaidCodes.Count();


            var result = await _controller.Delete(cId);

            Assert.Equal(beforeCount - 1, _context.PrepaidCodes.Count());

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact(DisplayName = "Currency delete non-existing code")]
        public async Task Currency_Delete_Code_Nonexistent()
        {
            var beforeCount = _context.PrepaidCodes.Count();

            var result = await _controller.Delete(new Guid());

            Assert.Equal(beforeCount, _context.PrepaidCodes.Count());

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Fact(DisplayName = "Successful prepaid card code redemtion")]
        public async Task User_use_code()
        {
            Guid code = Guid.NewGuid();
            string uid = "ec04ee08-f434-41d0-a208-15bd2dcb3389";
            // Add a code (non user)
            _context.PrepaidCodes.Add(new PrepaidCode { Amount = 10, CreationDate = DateTime.Now.AddDays(-1), Id = code });
            _context.Users.Add(new ApplicationUser { Id = uid });
            _context.SaveChanges();


            var m_User = new Mock<ClaimsPrincipal>();
            var m_Data = new Mock<ITempDataProvider>();
            m_User.Setup(t => t.Claims).Returns(new[] { new Claim(ClaimTypes.NameIdentifier, uid) });
            var httpContext = new DefaultHttpContext() { User = m_User.Object };
            var manageCtrl =
                new Controllers.ManageController(null, null, null, null, null, _context,
                    new Mock<IMemoryCache>().Object, new Mock<IStringLocalizer<Controllers.ManageController>>().Object)
                {
                    ControllerContext = new ControllerContext {HttpContext = httpContext},
                    TempData = new TempDataDictionary(httpContext, m_Data.Object)
                };

            // Actual test
            var money1 = _context.Users.Single(u => u.Id == uid).Coins;
            await manageCtrl.Redeem(new ViewModels.ManageViewModels.RedeemViewModel { Id = code.ToString() });
            var money2 = _context.Users.Single(u => u.Id == uid).Coins;

            Assert.Equal(money1 + 10, money2);
            Assert.NotNull(_context.PrepaidCodes.Single(c => c.Id == code).RedemptionDate);
            Assert.Equal(uid, _context.PrepaidCodes.Single(c => c.Id == code).Redeemer.Id);
        }
    }
}