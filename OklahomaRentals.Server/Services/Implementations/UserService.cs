using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.Repositories.Interfaces;

namespace OklahomaRentals.Server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync();

            return users.Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role,
                FullName = u.FullName
            });
        }

        public async Task<UserResponse?> GetByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                return null;

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                FullName = user.FullName
            };
        }

        public async Task<UserResponse> CreateAsync(UserResponse userRequest)
        {
            var entity = new Data.Entities.User
            {
                Email = userRequest.Email,
                Role = userRequest.Role,
                FullName = userRequest.FullName,
                PasswordHash = "" // Password handling should be added
            };

            var created = await _repository.CreateAsync(entity);

            return new UserResponse
            {
                Id = created.Id,
                Email = created.Email,
                Role = created.Role,
                FullName = created.FullName
            };
        }

        public async Task<UserResponse?> UpdateAsync(int id, UserResponse userRequest)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                return null;

            user.Email = userRequest.Email;
            user.Role = userRequest.Role;
            user.FullName = userRequest.FullName;

            var updated = await _repository.UpdateAsync(user);

            return new UserResponse
            {
                Id = updated.Id,
                Email = updated.Email,
                Role = updated.Role,
                FullName = updated.FullName
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}