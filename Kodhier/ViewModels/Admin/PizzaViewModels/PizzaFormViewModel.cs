using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kodhier.Models;

namespace Kodhier.ViewModels.Admin.PizzaViewModels
{
    public class PizzaFormViewModel
    {
        [Required]
        [Display(Name = "Pizza name lt_LT")]
        public string NameLt { get; set; }

        [Required]
        [Display(Name = "Pizza name en_US")]
        public string NameEn { get; set; }

        [Required]
        [Display(Name = "Path to the pizza image")]
        public string ImagePath { get; set; }

        [Required]
        [Display(Name = "Description lt_LT")]
        public string DescriptionLt { get; set; }

        [Required]
        [Display(Name = "Description en_US")]
        public string DescriptionEn { get; set; }

        [Required]
        [Display(Name = "Price category")]
        public int PriceCategoryId { get; set; }

        // For display
        public IEnumerable<PizzaPriceCategory> PriceCategories { get; set; }

        // For image selection
        public IEnumerable<string> ImageList { get; set; }
    }
}
