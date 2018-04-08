namespace Kodhier.ViewModels
{
    public class DropdownViewModel
    {
        public string Name { get; set; }
        public decimal Coins { get; set; }

        public DropdownViewModel(string name, decimal coins)
        {
            Name = name;
            Coins = coins;
        }
    }
}
