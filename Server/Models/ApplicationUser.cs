using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DentistryManagement.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }

        public int AffiliateId { get; set; }

        public Affiliate Affiliate { get; set; }

        public ICollection<TreatmentHistory> TreatmentHistories { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Schedule> Schedule { get; set; }
    }
}
