using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.Repositories.Interfaces;

namespace OklahomaRentals.Server.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repository;

        public AdminService(IAdminRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AdminResponse>> GetAllAsync()
        {
            var admins = await _repository.GetAllAsync();

            // Map entities to response DTOs
            return admins.Select(a => new AdminResponse
            {
                Id = a.Id,
                UserId = a.UserId,
                Username = a.Username
            });
        }

        public async Task<AdminResponse?> GetByIdAsync(int id)
        {
            var admin = await _repository.GetByIdAsync(id);

            if (admin == null)
                return null;

            return new AdminResponse
            {
                Id = admin.Id,
                UserId = admin.UserId,
                Username = admin.Username
            };
        }

        public async Task<AdminResponse> CreateAsync(AdminResponse adminRequest)
        {
            var entity = new Data.Entities.Admin
            {
                UserId = adminRequest.UserId,
                Username = adminRequest.Username
            };

            var created = await _repository.CreateAsync(entity);

            return new AdminResponse
            {
                Id = created.Id,
                UserId = created.UserId,
                Username = created.Username
            };
        }

        public async Task<AdminResponse?> UpdateAsync(int id, AdminResponse adminRequest)
        {
            var admin = await _repository.GetByIdAsync(id);
            if (admin == null)
                return null;

            admin.UserId = adminRequest.UserId;
            admin.Username = adminRequest.Username;

            var updated = await _repository.UpdateAsync(admin);

            return new AdminResponse
            {
                Id = updated.Id,
                UserId = updated.UserId,
                Username = updated.Username
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}