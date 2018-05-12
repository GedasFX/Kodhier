using System.Threading.Tasks;
using Kodhier.Data;
using Kodhier.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kodhier.Extensions
{
    public static class WebHostExtentions
    {
        public static async Task<IWebHost> SeedData(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KodhierDbContext>();

                await SeedDatabase(context);
            }
            return host;
        }

        private static async Task SeedDatabase(KodhierDbContext context)
        {
            if (!await context.PizzaPriceCategories.AnyAsync(d => d.Id == 0))
            {
                await context.PizzaPriceCategories.AddAsync(new PizzaPriceCategory() { Id = 0, Description = "Error category" });
            }
        }
    }
}
