using System;

namespace OklahomaRentals.Server.API.Response
{
    // What the API returns when a property is requested
    public class PropertyResponse
    {
        public int Id { get; set; }

        public string Address { get; set; }
        public DateTime NextAvailable { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Campus { get; set; } = string.Empty;

        public int Bedrooms { get; set; }

        public int Bathrooms { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsLeased { get; set; }
    }
}