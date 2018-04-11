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
    }
}
