using System;
using System.ComponentModel.DataAnnotations;
using Kodhier.Models;

namespace Kodhier.ViewModels.OrderViewModels
{
    public class OrderHistoryViewModel
    {
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Order status")]
        public OrderStatus Status { get; set; }

        public int Size { get; set; }
        public decimal Price { get; set; }
        public DateTime DateCreated { get; set; }
        public Pizza Pizza { get; set; }
    }
}
