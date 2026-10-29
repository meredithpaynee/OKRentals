using System.Collections.Generic;
using System.Threading.Tasks;
using OklahomaRentals.Server.API.Response;

namespace OklahomaRentals.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllAsync();
        Task<UserResponse?> GetByIdAsync(int id);
        Task<UserResponse> CreateAsync(UserResponse user);
        Task<UserResponse?> UpdateAsync(int id, UserResponse user);
        Task<bool> DeleteAsync(int id);
    }
}