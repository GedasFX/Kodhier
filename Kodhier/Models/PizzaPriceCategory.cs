using System.Collections.Generic;

namespace Kodhier.Models
{
    public class PizzaPriceCategory
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Description;
        }

        public virtual List<PizzaPriceInfo> PizzaPriceInfos { get; set; }
        public virtual List<Pizza> Pizzas { get; set; }
    }
}
