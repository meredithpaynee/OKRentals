using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.Data.Entities
{
    public class Document
    {
        public int Id { get; set; }

        // Foreign Keys
        public int PropertyId { get; set; }
        public int TenantId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Type { get; set; } = string.Empty; // Lease, Agreement, etc.

        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public DateTime DateSubmitted { get; set; }

        public byte[]? PdfDocument { get; set; }

        // Navigation Properties
        public Property? Property { get; set; }
        public Tenant? Tenant { get; set; }
    }
}