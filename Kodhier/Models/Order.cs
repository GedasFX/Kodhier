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

        public DateTime PlacementDate { get; set; }     // When user adds item to cart
        public DateTime? PaymentDate { get; set; }      // When user checks out
        public DateTime? CookingDate { get; set; }      // When chef clicks assign pizza
        public DateTime? ReadyDate { get; set; }        // When chef clicks finish pizza
        public DateTime? DeliveryDate { get; set; }     // When delivery clicks assign pizza
        public DateTime? CompletionDate { get; set; }   // When delivery clicks delivered

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

        [ForeignKey("Deliveree")]
        public string DelivereeId { get; set; }
        public virtual ApplicationUser Deliveree { get; set; }

        public ColorCode DeliveryColor { get; set; }
    }
}
