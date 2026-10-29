using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace OklahomaRentals.Server.Services.Interfaces
{
    // interface defines what the service must implement, but not how it does it — that’s up to the concrete class that implements this interface
    // controller will depend on this interface, and we can swap out different implementations
    // (e.g., a mock service for testing, or a real service that talks to a database) without changing the controller code
    public interface IAuthService
    {
        // Stub for now, later will take a login request DTO and return a JWT token if successful, or throw an exception if not.
        Task<string> LoginAsync();
        // Stub for now, later will take a registration request DTO, create a new user in the database,
        // and return a JWT token or AuthResponse (which contains the JWT + some other useful info, user id etc) if successful, or throw an exception if not.
        Task<string> RegisterAsync();
    }
}
