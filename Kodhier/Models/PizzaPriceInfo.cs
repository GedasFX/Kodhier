using System.ComponentModel.DataAnnotations.Schema;

namespace Kodhier.Models
{
    public class PizzaPriceInfo
    {
        // Id = Pizza.Id
        public int Id { get; set; }

        public int Size { get; set; }
        public decimal Price { get; set; }

        [ForeignKey("PriceCategory")]
        public int PriceCategoryId { get; set; }
        public virtual PizzaPriceCategory PriceCategory { get; set; }

        public override string ToString()
        {
            return PriceCategory.Description;
        }
    }
}