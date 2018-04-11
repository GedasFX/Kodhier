using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "Lowest price")]
        public decimal MinPrice { get; set; }
    }
}
