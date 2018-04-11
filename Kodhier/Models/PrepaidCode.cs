using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kodhier.Models
{
    public class PrepaidCode
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? RedemptionDate { get; set; }

        [ForeignKey("Redeemer")]
        public string RedeemerId { get; set; }
        public virtual ApplicationUser Redeemer { get; set; }
    }
}
