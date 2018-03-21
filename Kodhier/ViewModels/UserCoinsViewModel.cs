namespace Kodhier.ViewModels
{
    public class UserCoinsViewModel
    {
        public string Name { get; set; }
        public decimal Coins { get; set; }

        public UserCoinsViewModel(string name, decimal coins)
        {
            Name = name;
            Coins = coins;
        }
    }
}
