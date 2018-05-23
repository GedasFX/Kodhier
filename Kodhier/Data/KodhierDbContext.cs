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

        public DbSet<News> News { get; set; }

        public KodhierDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PizzaPriceInfo>()
                .HasOne(ppi => ppi.PriceCategory)
                .WithMany(ppc => ppc.PizzaPriceInfos)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Pizza>()
                .HasOne(p => p.PriceCategory)
                .WithMany(p => p.Pizzas)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Pizza)
                .WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
