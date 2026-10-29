using OklahomaRentals.Server.API.Requests;
using System.ComponentModel.DataAnnotations;

namespace OklahomaRentals.Server.API.Requests
{
    public class TenantRequest
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }
    }
}