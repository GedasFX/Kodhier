using Kodhier.Models;
using System.Collections.Generic;

namespace Kodhier.ViewModels.Admin.PizzaViewModels
{
    public class PizzaDetailsViewModel
    {
        public string NameLt { get; set; }
        public string NameEn { get; set; }

        public string DescriptionLt { get; set; }
        public string DescriptionEn { get; set; }

        public string ImagePath { get; set; }

        public IEnumerable<PizzaPriceInfo> Prices { get; set; }
    }
}
