using Kodhier.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kodhier.Data
{
    public class KodhierDbContext : DbContext
    {
        public DbSet<Pizza> Pizzas { get; set; }

        public KodhierDbContext(DbContextOptions<KodhierDbContext> options) : base(options)
        {

        }
    }
}
