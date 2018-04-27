using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kodhier.Models
{
    public class Pizza
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }

        [ForeignKey("PriceCategory")]
        public int PriceCategoryId { get; set; }
        public virtual PizzaPriceCategory PriceCategory { get; set; }

        public virtual List<Order> Orders { get; set; }
    }
}
