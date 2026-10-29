using OklahomaRentals.Server.API.Response;
using System;

namespace OklahomaRentals.Server.API.Response
{
    // What the API returns for maintenance requests
    public class MaintenanceRequestResponse
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int PropertyId { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime DateRequested { get; set; }

        public DateTime? DateResolved { get; set; }
    }
}

