using System;

namespace Kodhier.Models
{
    public class Pizza
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Size { get; set; }

        public ApplicationUser Creator { get; set; }


    }
}
