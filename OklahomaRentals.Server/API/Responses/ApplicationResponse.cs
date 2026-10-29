using System;

namespace OklahomaRentals.Server.API.Response
{
    public class ApplicationResponse
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public DateTime MoveInDate { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}