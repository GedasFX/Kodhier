using System;

namespace Kodhier.Models
{
    public class PrepaidCode
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }

        public virtual ApplicationUser Redeemer { get; set; }
    }
}
