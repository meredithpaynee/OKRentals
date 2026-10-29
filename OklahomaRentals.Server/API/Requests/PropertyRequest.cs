using OklahomaRentals.Server.API.Requests;
using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.API.Requests
{
    public class PropertyRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Campus { get; set; } = string.Empty;

        [Range(0, 10)]
        public int Bedrooms { get; set; }

        public DateTime NextAvailable { get; set; }

        [Range(0, 10)]
        public int Bathrooms { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public int AdminId { get; set; }
    }
}