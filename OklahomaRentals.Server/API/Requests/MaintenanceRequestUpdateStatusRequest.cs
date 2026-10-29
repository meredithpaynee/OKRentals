using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.API.Requests
{
	public class MaintenanceRequestUpdateStatusRequest
	{
		[Required]
		public string Status { get; set; } = string.Empty; // Pending, Completed, Cancelled

        public DateTime? DateResolved { get; set; }
    }
}

