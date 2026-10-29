using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.Data.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        // Foreign Keys
        public int TenantId { get; set; }
        public int PropertyId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        // Make nullable - not paid yet
        public DateTime? PaymentDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AmountDue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AmountPaid { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Late, Partial

        // Navigation Properties
        public Tenant? Tenant { get; set; }
        public Property? Property { get; set; }
    }
}