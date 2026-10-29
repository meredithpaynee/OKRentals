using System;

namespace OklahomaRentals.Server.API.Response
{
    public class LeaseInformationResponse
    {
        public int TenantId { get; set; }
        public int PropertyId { get; set; }
        
        // Property Information
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        
        // Lease Dates
        public DateTime LeaseStartDate { get; set; }
        public DateTime? LeaseEndDate { get; set; }
        public bool IsActiveLease { get; set; }
        public int DaysRemainingInLease { get; set; }
        
        // Rent Information
        public decimal MonthlyRentAmount { get; set; }
        
        // Landlord Contact
        public string LandlordName { get; set; } = string.Empty;
        public string LandlordEmail { get; set; } = string.Empty;
        public string LandlordPhoneNumber { get; set; } = string.Empty;
    }
}
