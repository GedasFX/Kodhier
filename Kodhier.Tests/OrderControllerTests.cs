using Kodhier.Controllers;
using Kodhier.Data;
using Kodhier.Models;
using Kodhier.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Kodhier.Tests
{
    public class OrderControlerTests
    {
        KodhierDbContext _mock;

        OrderController _controller;

        public OrderControlerTests()
        {
            try { AutoMapperConfig.RegisterMappings(); } catch { };

            _mock = new KodhierDbContext(new DbContextOptionsBuilder<KodhierDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            GenMockData();

            var m_User = new Mock<ClaimsPrincipal>();
            m_User.Setup(t => t.Claims).Returns(new[] { new Claim(ClaimTypes.NameIdentifier, "ec04ee08-f434-41d0-a208-15bd2dcb3389") });
            _controller = new OrderController(_mock)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = m_User.Object } }
            };
        }

        private void GenMockData()
        {
            var uId = new Guid("ec04ee08-f434-41d0-a208-15bd2dcb3389");
            var pId = Guid.NewGuid();
            var user = new ApplicationUser() { Id = uId.ToString(), Age = new DateTime(), Email = "asdf@jkl.lt", FirstName = "ASdF", UserName = "AwDcV" };
            _mock.Users.Add(user);
            _mock.Users.Add(new ApplicationUser() { Id = Guid.NewGuid().ToString(), Age = new DateTime(), Email = "dummy@jkdl.lt", FirstName = "dummy", UserName = "dumb" });
            _mock.Pizzas.Add(new Pizza() { Creator = user, Id = pId, Name = "Havaian", Price = 46.5M });
            _mock.SaveChangesAsync();
        }

        [Fact(DisplayName = "Create method add order to db with correct data")]
        public async Task Create_Order_success()
        {
            var pizza = _mock.Pizzas.Single(e => e.Name == "Havaian");
            var user = _mock.Users.Single(e => e.UserName == "AwDcV");
            var order = new OrderViewModel() { Pizza = pizza, Quantity = 3, Size = 5 };
            if (!Validator.TryValidateObject(order, new ValidationContext(order), null, true)) ;
            _controller.ModelState.AddModelError("err", "Error");
            var res = await _controller.Create(pizza.Id, new OrderCreateViewModel { Order = order });

            Assert.IsType<RedirectToActionResult>(res);
            Assert.Equal("Index", ((RedirectToActionResult)res).ActionName);
        }

        [Fact(DisplayName = "Create method not add an order to db with forged or wrong data")]
        public async Task Create_Order_fail()
        {
            var pizza = _mock.Pizzas.Single(e => e.Name == "Havaian");
            var user = _mock.Users.Single(e => e.UserName == "AwDcV");
            var order = new OrderViewModel() { Pizza = pizza, Quantity = -9 };
            if (!Validator.TryValidateObject(order, new ValidationContext(order), null, true));
                _controller.ModelState.AddModelError("err", "Error");
            var res = await _controller.Create(pizza.Id, new OrderCreateViewModel { Order = order });

            Assert.IsType<ViewResult>(res);
        }
    }
}
