using System;

namespace Kodhier.Models
{
    public class Pizza
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }

        public virtual ApplicationUser Creator { get; set; }
    }
}
