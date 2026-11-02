using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.API.Requests
{
    public class ApplicationStatusRequest
    {
        // Pending, Approved or Declined
        [Required] public string Status { get; set; } = string.Empty;
    }
}
