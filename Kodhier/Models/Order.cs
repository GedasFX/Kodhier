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
        public DateTime? OvenDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public bool IsPaid { get; set; }
        public OrderStatus Status { get; set; }

        [ForeignKey("PizzaPriceCategory")]
        public int? PizzaPriceCategoryId { get; set; }
        public virtual PizzaPriceCategory PizzaPriceCategory { get; set; }

        [ForeignKey("Pizza")]
        public Guid? PizzaId { get; set; }
        public virtual Pizza Pizza { get; set; }

        [ForeignKey("Client")]
        public string ClientId { get; set; }
        public virtual ApplicationUser Client { get; set; }
        
    }
}
