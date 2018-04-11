﻿using Kodhier.Models;
using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "Price categories")]
        public PizzaPriceCategory PriceCategory { get; set; }

        [Display(Name = "Lowest price")]
        public decimal MinPrice { get; set; }
    }
}
