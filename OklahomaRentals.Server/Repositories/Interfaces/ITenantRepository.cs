using OklahomaRentals.Server.Data.Entities;

namespace OklahomaRentals.Server.Repositories.Interfaces
{
    public interface ITenantRepository
    {
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant?> GetByIdAsync(int id);
        Task<Tenant> CreateAsync(Tenant tenant);
        Task<Tenant> UpdateAsync(Tenant tenant);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}