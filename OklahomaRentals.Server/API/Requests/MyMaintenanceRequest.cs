using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.API.Requests
{
    // A maintenance request from the signed-in tenant. Tenant and property
    // come from their account, not from the request body.
    public class MyMaintenanceRequest
    {
        [Required, MaxLength(100)] public string Title { get; set; } = string.Empty;
        [Required, MaxLength(1000)] public string Description { get; set; } = string.Empty;
        [Required] public string Priority { get; set; } = "Medium";
    }
}
