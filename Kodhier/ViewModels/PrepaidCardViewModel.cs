using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kodhier.Models;

namespace Kodhier.ViewModels
{
    public class PrepaidCardViewModel
    {
        [Display(Name = "Code")]
        public Guid Id { get; set; }

        [Display(Name = "Creation date")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Redemption date")]
        public DateTime? RedemptionDate { get; set; }

        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Redeemer")]
        public ApplicationUser Redeemer { get; set; }

        public IEnumerable<PrepaidCardViewModel> Elements { get; set; }
    }
}
