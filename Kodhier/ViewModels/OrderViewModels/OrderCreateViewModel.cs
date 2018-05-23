using Kodhier.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kodhier.ViewModels.OrderViewModels
{
    public class OrderCreateViewModel
    {
        [Required (ErrorMessage = "The Quantity field is required.")]
        [Range(1, 100, ErrorMessage = "Quantity must be greater than 0 and must be a reasonable amount")]
        public int Quantity { get; set; }

        // Pizza details for displaying information about pizza
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }

        // Selceted price/size combo
        // User input.
        [Required]
        [Display(Name = "Size")]
        public int SizeId { get; set; }

        [MaxLength(255, ErrorMessage = "Comment cannot be longer than 255 characters")]
        public string Comment { get; set; }

        // Filled from controller
        public IEnumerable<PizzaPriceInfo> Prices { get; set; }

        [Display(Name = "Price")]
        public decimal MinPrice { get; set; }
    }
}
