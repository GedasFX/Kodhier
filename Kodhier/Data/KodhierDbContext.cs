using Kodhier.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kodhier.Data
{
    public class KodhierDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<PrepaidCode> PrepaidCodes { get; set; }

        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<PizzaPriceInfo> PizzaPriceInfo { get; set; }
        public DbSet<PizzaPriceCategory> PizzaPriceCategories { get; set; }

        public KodhierDbContext(DbContextOptions options) : base(options) { }
    }
}
