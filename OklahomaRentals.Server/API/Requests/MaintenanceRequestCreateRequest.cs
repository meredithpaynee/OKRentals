using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.API.Requests
{
    public class MaintenanceRequestCreateRequest
    {
        [Required] public int TenantId { get; set; }
        [Required] public int PropertyId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string RequestPriority { get; set; } = "Medium";
    }
}