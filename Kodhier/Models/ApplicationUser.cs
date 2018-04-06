using Microsoft.AspNetCore.Identity;
using System;

namespace Kodhier.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public decimal Coins { get; set; }
    }
}
