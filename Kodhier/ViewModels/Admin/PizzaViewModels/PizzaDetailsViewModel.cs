using Kodhier.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kodhier.ViewModels.Admin.PizzaViewModels
{
    public class PizzaDetailsViewModel
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

        public PizzaDetailsViewModel EnumeratePrices(IQueryable<PizzaPriceInfo> context)
        {
            Prices = context.Where(ppi => ppi.PriceCategoryId == PriceCategory.Id);
            return this;
        }
    }
}
