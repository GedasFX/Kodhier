using Kodhier.Controllers;
using Kodhier.Data;
using Kodhier.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kodhier.Tests
{
    public class OrderControlerTests
    {
        KodhierDbContext _mock = new KodhierDbContext(new DbContextOptionsBuilder<KodhierDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        public OrderControlerTests()
        {
            GenMockData();
        }

        private void GenMockData()
        {
            var uId = Guid.NewGuid();
            var pId = Guid.NewGuid();
            var user = new ApplicationUser() { Id = uId.ToString(), Age = new DateTime(), Email = "asdf@jkl.lt", FirstName = "ASdF", UserName = "AwDcV" };
            _mock.Users.Add(user);
            _mock.Users.Add(new ApplicationUser() { Id = Guid.NewGuid().ToString(), Age = new DateTime(), Email = "dummy@jkdl.lt", FirstName = "dummy", UserName = "dumb" });
            _mock.Pizzas.Add(new Pizza() { Creator = user, Id = pId, Name = "Havaian", Price = 46.5M, Size = 46 });
            _mock.SaveChangesAsync();
        }

        [Fact(DisplayName = "Create method add order to db with correct data")]
        public async Task Create_Order_success()
        {
            var pizza = _mock.Pizzas.Single(e => e.Name == "Havaian");
            var user = _mock.Users.Single(e => e.UserName == "AwDcV");
            var order = new Order() { Pizza = pizza, Client = user, Quantity = 3 };
            var controller = new OrderController(_mock);
            var res = await controller.Create(order);

            Assert.IsType<RedirectToActionResult>(res);
            Assert.Equal("Index", ((RedirectToActionResult)res).ActionName);
        }

        [Fact(DisplayName = "Create method not add an order to db with forged or wrong data")]
        public async Task Create_Order_fail()
        {
            var pizza = _mock.Pizzas.Single(e => e.Name == "Havaian");
            var user = _mock.Users.Single(e => e.UserName == "AwDcV");
            var order = new Order() { Pizza = pizza, Client = user, Quantity = -9 };
            var controller = new OrderController(_mock);
            var res = await controller.Create(order);

            Assert.IsType<ViewResult>(res);
        }
    }
}
