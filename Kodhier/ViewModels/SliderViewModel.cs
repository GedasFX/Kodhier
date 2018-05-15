using System;
using Kodhier.Models;

namespace Kodhier.ViewModels
{
    public class SliderViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }

        public bool IsActive { get; set; }

        public Guid? PizzaId { get; set; }
        public virtual Pizza Pizza { get; set; }
    }
}
