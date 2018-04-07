using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Kodhier.Models;

namespace Kodhier.ViewModels.PizzaViewModels
{
    public class PizzaViewModel
    {
        [Display(Name = "Pizza name")]
        public string Name { get; set; }

        [Display(Name = "Path to the pizza image")]
        public string ImagePath { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Price categories")]
        public PizzaPriceCategory PriceCategory { get; set; }

        public IEnumerable<PizzaPriceInfo> Prices { get; set; }
        public IEnumerable<PizzaPriceCategory> PriceCategories { get; set; }

        public PizzaViewModel EnumeratePrices(IQueryable<PizzaPriceInfo> context)
        {
            Prices = context.Where(ppi => ppi.PriceCategoryId == PriceCategory.Id);
            return this;
        }
    }
}
