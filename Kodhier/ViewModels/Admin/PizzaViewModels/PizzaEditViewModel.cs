using System.Collections.Generic;
using Kodhier.Models;
using System.ComponentModel.DataAnnotations;

namespace Kodhier.ViewModels.Admin.PizzaViewModels
{
    public class PizzaEditViewModel
    {
        [Display(Name = "Pizza name")]
        public string Name { get; set; }

        [Display(Name = "Path to the pizza image")]
        public string ImagePath { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Price categories")]
        public PizzaPriceCategory PriceCategory { get; set; }

        public IEnumerable<PizzaPriceCategory> PriceCategories { get; set; }
    }
}
