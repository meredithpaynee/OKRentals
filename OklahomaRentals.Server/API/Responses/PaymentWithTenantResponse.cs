using System;

namespace OklahomaRentals.Server.API.Response
{
    public class PaymentWithTenantResponse
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int PropertyId { get; set; }
        
        // Tenant Information
        public string TenantName { get; set; } = string.Empty;
        public string TenantEmail { get; set; } = string.Empty;
        public string TenantPhoneNumber { get; set; } = string.Empty;
        
        // Payment Information
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal OutstandingBalance { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
