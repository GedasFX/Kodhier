using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kodhier.ViewModels
{
    public class ConfirmCheckoutViewModel
    {
        [Required(ErrorMessage = "You must provide a delivery addess")]
        public string ConfirmAddress { get; set; }

        [Required(ErrorMessage = "You must provide a phone number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public decimal Price { get; set; }
        public IEnumerable<CheckoutViewModel> CheckoutList { get; set; }
    }
}
