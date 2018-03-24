using System;

namespace Kodhier.ViewModels
{
    public class OrderCreateViewModel
    {
        public OrderViewModel Order { get; set; }

        public string Name { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}
