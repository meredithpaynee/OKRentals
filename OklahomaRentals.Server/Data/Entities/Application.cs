using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.Data.Entities
{
    public class Application
    {
        public int Id { get; set; }

        // Foreign Key
        public int PropertyId { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public DateTime MoveInDate { get; set; }

        [Required]
        public DateTime DateSubmitted { get; set; }

        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        public byte[]? PdfDocument { get; set; }

        // Navigation Property
        public Property? Property { get; set; }
    }
}