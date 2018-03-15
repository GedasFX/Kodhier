using System;
using System.ComponentModel.DataAnnotations;

namespace Kodhier.Areas.Admin.ViewModels
{
    public class PizzaViewModel
    {
        [Required]
        [Display(Name = "Pizza name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Path to the pizza image")]
        public string ImagePath { get; set; }

        [Required]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        public Guid Id { get; set; }
    }
}
