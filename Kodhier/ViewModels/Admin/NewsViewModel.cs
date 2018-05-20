using System;
using System.Collections.Generic;
using Kodhier.Models;

namespace Kodhier.ViewModels.Admin
{
    public class NewsViewModel
    {
        public News[] Slides { get; set; }
        public IEnumerable<Pizza> Pizzas { get; set; }
        public IEnumerable<string> Images { get; set; }

        public Guid? PizzaId { get; set; }

        public string IsActive { get; set; }

        public string CaptionLt { get; set; }
        public string CaptionEn { get; set; }
        public string TitleLt { get; set; }
        public string TitleEn { get; set; }

        public string ImagePath { get; set; }
    }
}
