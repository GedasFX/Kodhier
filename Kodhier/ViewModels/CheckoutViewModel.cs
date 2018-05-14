using System;

namespace Kodhier.ViewModels
{
    public class CheckoutViewModel
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public int Size { get; set; }
        public string Comment { get; set; }

        public string Name { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}
