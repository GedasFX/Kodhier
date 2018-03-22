using Kodhier.ViewModels;
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
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price field must be a positive number")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Details")]
        public string Details { get; set; }

        public Guid Id { get; set; }
        public OrderViewModel Order { get; set; }
    }
}
