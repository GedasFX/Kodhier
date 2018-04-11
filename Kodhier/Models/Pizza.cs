using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kodhier.Models
{
    public class Pizza
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }

        public virtual PizzaPriceCategory PriceCategory { get; set; }
    }
}
