using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.Data.Entities
{
    public class Property
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Campus { get; set; } = string.Empty;

        [Range(0, 10)]
        public int Bedrooms { get; set; }

        [Range(0, 10)]
        public int Bathrooms { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        public bool IsLeased { get; set; }

        public DateTime NextAvailable { get; set; }

        // Foreign Key
        public int AdminId { get; set; }

        // Navigation Properties
        public Admin? Admin { get; set; }
        public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}