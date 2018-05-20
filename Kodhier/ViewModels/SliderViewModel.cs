using Kodhier.Models;

namespace Kodhier.ViewModels
{
    public class SliderViewModel
    {
        public string Title { get; set; }
        public string Caption { get; set; }

        public decimal Price { get; set; }

        public string PizzaImgPath { get; set; }
        public Pizza Pizza { get; set; }
    }
}
