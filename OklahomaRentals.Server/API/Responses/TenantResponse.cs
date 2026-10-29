using System;

namespace OklahomaRentals.Server.API.Response
{
    // What the API is allowed to return about a tenant, removed password field for security 
    public class TenantResponse
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }
    }
}
