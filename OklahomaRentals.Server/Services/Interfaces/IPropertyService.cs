using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.API.Requests;

namespace OklahomaRentals.Server.Services.Interfaces
{
    // interface defines what the service must implement, but not how it does it — that’s up to the concrete class that implements this interface
    // controller will depend on this interface, and we can swap out different implementations
    // (e.g., a mock service for testing, or a real service that talks to a database) without changing the controller code
    public interface IPropertyService
    {
        // returning a list of properties, and a single property by ID. We can add more methods later for creating/updating/deleting properties as needed
        Task<IEnumerable<PropertyResponse>> GetAllAsync();

        // returning a single property by ID, and we return null if it’s not found (hence the nullable PropertyResponse?).
        // The controller can then handle this case and return a 404 Not Found if needed
        Task<PropertyResponse?> GetByIdAsync(int id);

        Task<PropertyResponse> CreateAsync(PropertyRequest property);

        Task<PropertyResponse?> UpdateAsync(int id, PropertyRequest property);

        Task<PropertyResponse?> UpdateDescriptionAsync(int id, string description);

        Task<PropertyResponse?> UpdateAvailabilityAsync(int id, bool isLeased);

        Task<PropertyResponse> CreateFillerTempProperty();
        
        Task<bool> DeleteAsync(int id);

        Task<PropertyResponse?> GetByNameAsync(string name);
    }
}
