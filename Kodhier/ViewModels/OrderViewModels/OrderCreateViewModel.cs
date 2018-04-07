using Kodhier.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kodhier.ViewModels.OrderViewModels
{
    public class OrderCreateViewModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        // Pizza details for displaying information about pizza
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }

        // Selceted price/size combo
        // User input.
        [Required]
        public PizzaPriceInfo Price { get; set; }
        public string Comment { get; set; }

        // Filled from controller
        public IEnumerable<PizzaPriceInfo> Prices { get; set; }
    }
}
