using Kodhier.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public IEnumerable<PizzaPriceInfo> Prices { get; set; }
    }
}
