using System;
using System.Threading.Tasks;
using OklahomaRentals.Server.Services.Interfaces;


namespace OklahomaRentals.Server.Services.Implementations
{
    // compiles, returns placeholder strings for now.
    public class AuthService : IAuthService
    {
        public Task<string> LoginAsync()
        {
            return Task.FromResult("Not implemented");
        }

        public Task<string> RegisterAsync()
        {
            return Task.FromResult("Not implemented");
        }
    }
}
