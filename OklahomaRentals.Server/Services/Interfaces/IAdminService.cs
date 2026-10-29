using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OklahomaRentals.Server.API.Response;

namespace OklahomaRentals.Server.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<AdminResponse>> GetAllAsync();

        Task<AdminResponse?> GetByIdAsync(int id);

        Task<AdminResponse> CreateAsync(AdminResponse Admin);
        Task<AdminResponse?> UpdateAsync(int id, AdminResponse admin);
        Task<bool> DeleteAsync(int id);
    }
}
