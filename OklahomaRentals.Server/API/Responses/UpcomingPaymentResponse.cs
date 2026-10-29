using System;

namespace OklahomaRentals.Server.API.Response
{
    public class UpcomingPaymentResponse
    {
        public int PaymentId { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsOverdue { get; set; }
        public int DaysUntilDue { get; set; } // Negative if overdue
    }
}
