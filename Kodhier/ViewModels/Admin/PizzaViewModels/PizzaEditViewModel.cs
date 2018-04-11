using System.Collections.Generic;
using Kodhier.Models;
using System.ComponentModel.DataAnnotations;

namespace Kodhier.ViewModels.Admin.PizzaViewModels
{
    public class PizzaEditViewModel
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
        [Display(Name = "Price category")]
        public int PriceCategoryId { get; set; }

        public IEnumerable<PizzaPriceCategory> PriceCategories { get; set; }
    }
}
