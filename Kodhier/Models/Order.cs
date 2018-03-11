using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kodhier.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public virtual Pizza Pizza { get; set; }
        public virtual PaymentType PaymentType { get; set; }

        public virtual ApplicationUser Client { get; set; }
    }
}
