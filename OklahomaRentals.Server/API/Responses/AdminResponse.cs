using System;

namespace OklahomaRentals.Server.API.Response
{
    public class AdminResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
