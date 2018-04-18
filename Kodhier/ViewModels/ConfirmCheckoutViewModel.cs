using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kodhier.ViewModels
{
    public class ConfirmCheckoutViewModel
    {
		public String ConfirmAddress { get; set; }

		public decimal Price { get; set; }
		public IEnumerable<CheckoutViewModel> CheckoutList { get; set; }
	}
}
