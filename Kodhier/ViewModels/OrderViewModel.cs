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
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }

        [Required]
        [ValidValues(new[] { 22, 23 }, ErrorMessage = "Invalid pizza size")]
        [Range(0, int.MaxValue, ErrorMessage = "Size must be a positive number")]
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
