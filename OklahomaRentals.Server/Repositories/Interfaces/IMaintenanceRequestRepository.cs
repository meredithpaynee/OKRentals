using OklahomaRentals.Server.Data.Entities;

namespace OklahomaRentals.Server.Repositories.Interfaces
{
    public interface IMaintenanceRequestRepository
    {
        Task<IEnumerable<MaintenanceRequest>> GetAllAsync();
        Task<MaintenanceRequest?> GetByIdAsync(int id);
        Task<MaintenanceRequest> CreateAsync(MaintenanceRequest maintenanceRequest);
        Task<MaintenanceRequest> UpdateAsync(MaintenanceRequest maintenanceRequest);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}