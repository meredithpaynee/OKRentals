using System;

namespace OklahomaRentals.Server.API.Response
{
    public class TenantAccountSummaryResponse
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        
        // Account Balance Information
        public decimal TotalDue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal CurrentBalance { get; set; } // Amount still owed
        
        // Payment Status
        public int TotalPayments { get; set; }
        public int PaidPayments { get; set; }
        public int OutstandingPayments { get; set; }
        public int OverduePayments { get; set; }
        
        // Upcoming Information
        public DateTime? NextPaymentDueDate { get; set; }
        public decimal? NextPaymentAmount { get; set; }
        public bool HasOutstandingBalance { get; set; }
        public bool HasOverduePayments { get; set; }
    }
}
