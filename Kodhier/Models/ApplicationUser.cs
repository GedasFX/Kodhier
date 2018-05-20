using Microsoft.AspNetCore.Identity;

namespace Kodhier.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public decimal Coins { get; set; }

        public string Address { get; set; }

        public bool EmailSendUpdates { get; set; }
        public bool EmailSendPromotional { get; set; }
    }
}