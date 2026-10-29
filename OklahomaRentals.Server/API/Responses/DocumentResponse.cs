using System;

namespace OklahomaRentals.Server.API.Response
{
    public class DocumentResponse
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int TenantId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime DateSubmitted { get; set; }
    }
}