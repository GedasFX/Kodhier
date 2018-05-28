using System.ComponentModel.DataAnnotations;
using Kodhier.Models;

namespace Kodhier.ViewModels.Admin.PizzaViewModels
{
    public class PizzaViewModel
    {
        [Display(Name = "Pizza name")]
        public string Name { get; set; }

        [Display(Name = "Path to the pizza image")]
        public string ImagePath { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Lowest price")]
        public PizzaPriceInfo[] PriceInfo { get; set; }

        public bool IsDepricated { get; set; }
    }
}
