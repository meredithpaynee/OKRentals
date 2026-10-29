namespace OklahomaRentals.Server.API.Response
{
    public class TenantOutstandingBalanceResponse
    {
        public int TenantId { get; set; }
        public int PropertyId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string TenantEmail { get; set; } = string.Empty;
        public string TenantPhoneNumber { get; set; } = string.Empty;
        public decimal TotalOutstandingBalance { get; set; }
        public int OverduePaymentCount { get; set; }
        public DateTime? EarliestOverdueDate { get; set; }
    }
}
