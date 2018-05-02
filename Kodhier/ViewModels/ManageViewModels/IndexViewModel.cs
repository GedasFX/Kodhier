using System;
using System.ComponentModel.DataAnnotations;

namespace Kodhier.ViewModels.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public bool EmailSendUpdates { get; set; }
        public bool EmailSendPromotional { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth date")]
        public DateTime? BirthDate { get; set; }

        public string StatusMessage { get; set; }
    }
}
