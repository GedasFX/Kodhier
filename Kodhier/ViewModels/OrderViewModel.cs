using Kodhier.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kodhier.ViewModels
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        [ValidValues(new[] { 20, 30, 50 }, ErrorMessage = "Invalid pizza size")]
        public int Size { get; set; }

        public bool IsPaymentSuccessful { get; set; }
        public bool IsFinished { get; set; }

        public Pizza Pizza { get; set; }

        class ValidValuesAttribute : ValidationAttribute
        {
            int[] values;
            public ValidValuesAttribute(int[] values)
            {
                this.values = values;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (Array.Exists(values, a => (int)value == a))
                    return ValidationResult.Success;
                return new ValidationResult(ErrorMessage);
            }
        }

    }
}
