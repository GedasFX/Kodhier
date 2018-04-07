using System.ComponentModel.DataAnnotations;
using Kodhier.Models;

namespace Kodhier.ViewModels.OrderViewModels
{
    public class OrderViewModel
    {
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Payment sucessful")]
        public bool IsPaid { get; set; }

        [Display(Name = "Order status")]
        public OrderStatus Status { get; set; }
    }
}
