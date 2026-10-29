using System.Collections.Generic;
using System.Threading.Tasks;
using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.API.Requests;

namespace OklahomaRentals.Server.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task<IEnumerable<MaintenanceRequestResponse>> GetAllAsync();
        Task<MaintenanceRequestResponse?> GetByIdAsync(int id);
        Task<MaintenanceRequestResponse> CreateAsync(MaintenanceRequestCreateRequest request);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<MaintenanceRequestResponse>> GetMaintenanceRequestsByTenantAsync(int tenantId);
        Task<IEnumerable<MaintenanceRequestResponse>> GetMaintenanceRequestsByPropertyAsync(int propertyId);
    }
}