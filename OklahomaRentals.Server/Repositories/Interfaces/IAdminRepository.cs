using OklahomaRentals.Server.Data.Entities;

namespace OklahomaRentals.Server.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<IEnumerable<Admin>> GetAllAsync();
        Task<Admin?> GetByIdAsync(int id);
        Task<Admin> CreateAsync(Admin Admin);
        Task<Admin> UpdateAsync(Admin Admin);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}