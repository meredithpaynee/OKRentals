using System;

namespace OklahomaRentals.Server.API.Response
{
    public class ApplicationResponse
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public DateTime MoveInDate { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string FileName { get; set; } = string.Empty;

        // Filled in for applications submitted through the website form.
        public string? PropertyName { get; set; }
        public string? DriversLicense { get; set; }
        public string? College { get; set; }
        public string? Major { get; set; }
        public DateTime? ExpectedGraduation { get; set; }
        public string? LeaseTerm { get; set; }
        public ResidenceResponse? CurrentResidence { get; set; }
        public ResidenceResponse? PreviousResidence { get; set; }
        public bool? Evicted { get; set; }
        public bool? BrokenLease { get; set; }
        public bool? Bankruptcy { get; set; }
        public bool? Convicted { get; set; }
        public string? Explanation { get; set; }
    }

    public class ResidenceResponse
    {
        public string? Address { get; set; }
        public string? Landlord { get; set; }
        public string? LandlordContact { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? ReasonForLeaving { get; set; }
    }
}