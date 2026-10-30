using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.API.Requests
{
    // The public "Apply to rent" form. Dates are kept as the yyyy-mm-dd strings
    // the browser sends, since they're only shown in the email.
    public class RentalApplicationRequest
    {
        [Required, MaxLength(200)] public string Name { get; set; } = string.Empty;
        [Required, EmailAddress, MaxLength(255)] public string Email { get; set; } = string.Empty;
        [Required, MaxLength(30)] public string Phone { get; set; } = string.Empty;
        [Required, MaxLength(10)] public string DateOfBirth { get; set; } = string.Empty;
        [MaxLength(50)] public string? DriversLicense { get; set; }

        [MaxLength(200)] public string? College { get; set; }
        [MaxLength(200)] public string? Major { get; set; }
        [MaxLength(10)] public string? ExpectedGraduation { get; set; }

        [Required, MaxLength(200)] public string Property { get; set; } = string.Empty;
        [MaxLength(10)] public string? LeaseStart { get; set; }
        [MaxLength(50)] public string? LeaseTerm { get; set; }

        public ResidenceHistory? CurrentResidence { get; set; }
        public ResidenceHistory? PreviousResidence { get; set; }

        [MaxLength(3)] public string? Evicted { get; set; }
        [MaxLength(3)] public string? BrokenLease { get; set; }
        [MaxLength(3)] public string? Bankruptcy { get; set; }
        [MaxLength(3)] public string? Convicted { get; set; }
        [MaxLength(2000)] public string? Explanation { get; set; }

        // Hidden field that people never see; bots that fill every input do.
        public string? Website { get; set; }
    }

    public class ResidenceHistory
    {
        [MaxLength(300)] public string? Address { get; set; }
        [MaxLength(200)] public string? Landlord { get; set; }
        [MaxLength(200)] public string? LandlordContact { get; set; }
        [MaxLength(10)] public string? From { get; set; }
        [MaxLength(10)] public string? To { get; set; }
        [MaxLength(500)] public string? ReasonForLeaving { get; set; }
    }
}
