using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.API.Requests
{
    public class PropertyDescriptionUpdateRequest
    {
        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;
    }
}