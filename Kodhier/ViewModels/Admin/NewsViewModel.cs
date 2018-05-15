using System;
using System.Collections.Generic;
using Kodhier.Models;

namespace Kodhier.ViewModels.Admin
{
    public class NewsViewModel
    {
        public News[] Slides { get; set; }
        public IEnumerable<Pizza> Pizzas { get; set; }

        public string Title { get; set; }
        public string Caption { get; set; }
        public Guid? PizzaId { get; set; }

        public string IsActive { get; set; }
    }
}
