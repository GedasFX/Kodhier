using Kodhier.Controllers;
using Kodhier.Data;
using Kodhier.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kodhier.ViewModels.OrderViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Xunit;

namespace Kodhier.Tests
{
    public class OrderControlerTests
    {
        readonly KodhierDbContext _context;
        readonly OrderController _controller;

        public OrderControlerTests()
        {
            try { AutoMapperConfig.RegisterMappings(); } catch { /* If failed, means already initialized */ }

            _context = new KodhierDbContext(new DbContextOptionsBuilder<KodhierDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            GenMockData();

            var m_User = new Mock<ClaimsPrincipal>();
            var m_Data = new Mock<ITempDataProvider>();
            var m_localizer = new Mock<IStringLocalizer<OrderController>>();
            var httpContext = new DefaultHttpContext { User = m_User.Object };
            m_User.Setup(t => t.Claims).Returns(new[] { new Claim(ClaimTypes.NameIdentifier, "ec04ee08-f434-41d0-a208-15bd2dcb3389") });
            _controller = new OrderController(_context, m_localizer.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext },
                TempData = new TempDataDictionary(httpContext, m_Data.Object)
            };
        }

        private void GenMockData()
        {
            var uId = new Guid("ec04ee08-f434-41d0-a208-15bd2dcb3389");
            var pId = Guid.NewGuid();
            var ppc = new PizzaPriceCategory { Description = "LOL", Id = 1 };
            var user = new ApplicationUser { Id = uId.ToString(), Email = "asdf@jkl.lt", UserName = "AwDcV" };
            _context.Users.Add(user);
            _context.Users.Add(new ApplicationUser { Id = Guid.NewGuid().ToString(), Email = "dummy@jkdl.lt", UserName = "dumb" });
            _context.PizzaPriceCategories.Add(ppc);
            _context.SaveChanges();
            _context.PizzaPriceInfo.Add(new PizzaPriceInfo { Id = 1, Price = 6.05m, PriceCategory = ppc, Size = 32 });
            _context.Pizzas.Add(new Pizza { Id = pId, NameEn = "Havaian", PriceCategory = _context.PizzaPriceCategories.Single(c => c.Id == 1) });
            _context.SaveChangesAsync();
        }

        [Fact(DisplayName = "Create method add order to db with correct data")]
        public async Task Create_Order_success()
        {
            var pizza = _context.Pizzas.Single(e => e.NameEn == "Havaian");
            var order = new OrderCreateViewModel { Name = pizza.NameEn, Description = pizza.DescriptionLt, ImagePath = pizza.ImagePath, Comment = "dad", Quantity = 3, SizeId = 1 };
            if (!Validator.TryValidateObject(order, new ValidationContext(order), null, true))
                _controller.ModelState.AddModelError("err", "Error");
            var res = await _controller.Create(pizza.NameEn, order);

            Assert.IsType<RedirectToActionResult>(res);
            Assert.Equal("Index", ((RedirectToActionResult)res).ActionName);
        }

        [Fact(DisplayName = "Create method not add an order to db with forged or wrong data")]
        public async Task Create_Order_fail()
        {
            var pizza = _context.Pizzas.Single(e => e.NameEn == "Havaian");
            var order = new OrderCreateViewModel { Name = pizza.NameEn, Description = pizza.DescriptionLt, ImagePath = pizza.ImagePath, Comment = "dad", Quantity = -1, SizeId = 1 };
            if (!Validator.TryValidateObject(order, new ValidationContext(order), null, true))
                _controller.ModelState.AddModelError("err", "Error");
            var res = await _controller.Create(pizza.NameEn, order);

            Assert.IsType<ViewResult>(res);

            order = new OrderCreateViewModel { Name = pizza.NameEn, Description = pizza.DescriptionLt, ImagePath = pizza.ImagePath, Comment = "dad", Quantity = 3, SizeId = 2 };
            if (!Validator.TryValidateObject(order, new ValidationContext(order), null, true))
                _controller.ModelState.AddModelError("err", "Error");
            res = await _controller.Create(pizza.NameEn, order);

            Assert.IsType<ViewResult>(res);
        }

        [Fact(DisplayName = "Instead of making new order, system updates a preexisting unpaid order.")]
        public async Task CreateOrderAlreadyExists()
        {
            var pizza = _context.Pizzas.Single(e => e.NameEn == "Havaian");
            var order = new OrderCreateViewModel { Name = pizza.NameEn, Description = pizza.DescriptionLt, ImagePath = pizza.ImagePath, Comment = "dad", Quantity = 3, SizeId = 1 };
            if (!Validator.TryValidateObject(order, new ValidationContext(order), null, true))
                _controller.ModelState.AddModelError("err", "Error");

            var res = await _controller.Create(pizza.NameEn, order);
        }
    }
}
