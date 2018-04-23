using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kodhier.ViewModels
{
    public class ConfirmCheckoutViewModel
    {
        [Required]
		public string ConfirmAddress { get; set; }

		public decimal Price { get; set; }
		public IEnumerable<CheckoutViewModel> CheckoutList { get; set; }
	}
}
