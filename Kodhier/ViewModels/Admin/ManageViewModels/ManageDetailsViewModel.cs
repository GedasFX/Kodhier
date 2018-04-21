using System.Collections.Generic;

namespace Kodhier.ViewModels.Admin.ManageViewModels
{
    public class ManageDetailsViewModel
    {
        public string Username { get; set; }
        public IEnumerable<string> UserRoles { get; set; }

        public string NewRole { get; set; }
        public IEnumerable<string> AllRoles { get; set; }
    }
}
