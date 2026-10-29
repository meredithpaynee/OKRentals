using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.Data.Entities
{
	public class MaintenanceRequest
	{
		public int Id { get; set; }

		// Foreign Keys
		public int TenantId { get; set; }
		public int PropertyId { get; set; }

		[Required]
		[MaxLength(100)]
		public string Title { get; set; } = string.Empty;

		[Required]
		[MaxLength(1000)]
		public string Description { get; set; } = string.Empty;

		[MaxLength(50)]
		public string Status { get; set; } = "Pending";

		[Required]
		public DateTime DateRequested { get; set; }

		// Make optional - only set when resolved
		public DateTime? DateResolved { get; set; }

		// Navigation Properties
		public Tenant? Tenant { get; set; }
		public Property? Property { get; set; }
	}
}