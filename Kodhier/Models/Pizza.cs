using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kodhier.Models
{
    public class Pizza
    {
        public Guid Id { get; set; }
        public string NameLt { get; set; }
        public string NameEn { get; set; }
        public string ImagePath { get; set; }
        public string DescriptionLt { get; set; }
        public string DescriptionEn { get; set; }

        // Pizza is deleted, but some elements in db (Orders) still reference it.
        public bool IsDepricated { get; set; }

        [ForeignKey("PriceCategory")]
        public int PriceCategoryId { get; set; }
        public virtual PizzaPriceCategory PriceCategory { get; set; }

        public virtual List<Order> Orders { get; set; }
    }
}
