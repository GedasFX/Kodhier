using Kodhier.Models;
using System;
using System.ComponentModel.DataAnnotations;

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

        public bool IsPaid { get; set; }

        public Pizza Pizza { get; set; }

        private class ValidValuesAttribute : ValidationAttribute
        {
            readonly int[] _values;
            public ValidValuesAttribute(int[] values)
            {
                _values = values;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext) => 
                Array.Exists(_values, a => (int)value == a) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }

    }
}
