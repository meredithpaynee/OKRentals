using OklahomaRentals.Server.API.Response;
using System;

namespace OklahomaRentals.Server.API.Response
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int PropertyId { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}