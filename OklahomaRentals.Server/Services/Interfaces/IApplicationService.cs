using System.Collections.Generic;
using System.Threading.Tasks;
using OklahomaRentals.Server.API.Requests;
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

        // Saves an application from the public website form. Throws
        // ArgumentException when the property or a date can't be resolved.
        Task<ApplicationResponse> SubmitAsync(RentalApplicationRequest request);

        Task<ApplicationResponse?> UpdateStatusAsync(int id, string status);
    }
}