using Kodhier.Models;

namespace Kodhier.ViewModels
{
    public class DeliveryViewModel
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
        public int Size { get; set; }
        public string Comment { get; set; }

        public string Name { get; set; }
        public string ImagePath { get; set; }

        public string DeliveryAddress { get; set; }
        public ColorCode DeliveryColor { get; set; }
        public string PhoneNumber { get; set; }
    }
}
