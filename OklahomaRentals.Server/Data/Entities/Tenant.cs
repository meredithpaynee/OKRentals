using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.Data.Entities
{
    public class Tenant
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public User User { get; set; } = null!;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public DateTime LeaseStartDate { get; set; }

        public DateTime? LeaseEndDate { get; set; }

        public Property? Property { get; set; }
        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}