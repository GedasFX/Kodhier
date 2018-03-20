using System;

namespace Kodhier.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }
        public int Size { get; set; }
        public string Comment { get; set; }

        public DateTime PlacementDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CookingDate { get; set; }
        public DateTime OvenDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public OrderStatus Status { get; set; }

        public virtual Pizza Pizza { get; set; }
        public virtual ApplicationUser Client { get; set; }
    }
}
