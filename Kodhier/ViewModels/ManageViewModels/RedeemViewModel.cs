using System;
using System.ComponentModel.DataAnnotations;

namespace Kodhier.ViewModels.ManageViewModels
{
    public class RedeemViewModel
    {
        [GuidValidation(ErrorMessage = "Incorrect input format.")]
        public string Id { get; set; }
    }

    class GuidValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return Guid.TryParse((string)value, out var o) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
