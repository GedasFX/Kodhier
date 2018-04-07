using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kodhier.Models;

namespace Kodhier.ViewModels.PizzaViewModels
{
    public class PizzaCreateViewModel
    {
        [Required]
        [Display(Name = "Pizza name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Path to the pizza image")]
        public string ImagePath { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Price categories")]
        public PizzaPriceCategory PriceCategory { get; set; }

        public IEnumerable<PizzaPriceInfo> Prices { get; set; }
    }
}
