﻿using System;

namespace Kodhier.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public int Size { get; set; }
        public bool IsPaymentSuccessful { get; set; }
        public bool IsFinished { get; set; }

        public virtual Pizza Pizza { get; set; }
        public virtual PaymentType PaymentType { get; set; }
        public virtual ApplicationUser Client { get; set; }
    }
}
