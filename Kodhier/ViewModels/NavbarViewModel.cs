namespace Kodhier.ViewModels
{
    public class NavbarViewModel 
    {
        public int Quantity { get; set; }

        public NavbarViewModel(int quantity)
        {
            Quantity = quantity;
        }
    }
}
