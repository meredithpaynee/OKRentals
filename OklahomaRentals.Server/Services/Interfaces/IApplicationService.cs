using System.Collections.Generic;
using System.Threading.Tasks;
using OklahomaRentals.Server.API.Response;

namespace OklahomaRentals.Server.Services.Interfaces
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationResponse>> GetAllAsync();
        Task<ApplicationResponse?> GetByIdAsync(int id);
        Task<ApplicationResponse> CreateAsync(ApplicationResponse application);
        Task<ApplicationResponse?> UpdateAsync(int id, ApplicationResponse application);
        Task<bool> DeleteAsync(int id);
    }
}