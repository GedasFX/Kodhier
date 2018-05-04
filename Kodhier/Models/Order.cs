using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kodhier.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }
        public int Size { get; set; }
        public decimal Price { get; set; }

        public string Comment { get; set; }

        public string DeliveryAddress { get; set; }

        public DateTime PlacementDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? CookingDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        public bool IsPaid { get; set; }
        public OrderStatus Status { get; set; }

        [ForeignKey("Pizza")]
        public Guid? PizzaId { get; set; }
        public virtual Pizza Pizza { get; set; }

        [ForeignKey("Client")]
        public string ClientId { get; set; }
        public virtual ApplicationUser Client { get; set; }

        [ForeignKey("Chef")]
        public string ChefId { get; set; }
        public virtual ApplicationUser Chef { get; set; }
    }
}
