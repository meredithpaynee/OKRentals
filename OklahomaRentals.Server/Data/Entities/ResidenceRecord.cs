using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OklahomaRentals.Server.Data.Entities
{
    // A previous or current home listed on a rental application.
    // Stored as columns on the Applications table (e.g. CurrentResidence_Address).
    [Owned]
    public class ResidenceRecord
    {
        [MaxLength(300)] public string? Address { get; set; }
        [MaxLength(200)] public string? Landlord { get; set; }
        [MaxLength(200)] public string? LandlordContact { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        [MaxLength(500)] public string? ReasonForLeaving { get; set; }
    }
}
