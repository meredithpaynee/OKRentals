using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.Data.Entities
{
    public class Admin
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(45)]
        public string Username { get; set; } = string.Empty;

        public User User { get; set; } = null!;

        // Navigation Properties
        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}