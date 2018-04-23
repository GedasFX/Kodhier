using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kodhier.Models;

namespace Kodhier.ViewModels.OrderViewModels
{
    public class OrderViewModel
    {
        [Display(Name = "Pizza name")]
        public string Name { get; set; }

        [Display(Name = "Path to the pizza image")]
        public string ImagePath { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Price information")]
        public IEnumerable<PizzaPriceInfo> PriceInfo { get; set; }
    }
}
