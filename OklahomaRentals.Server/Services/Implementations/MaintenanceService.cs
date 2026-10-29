using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.API.Requests;
using OklahomaRentals.Server.Repositories.Interfaces;
using OklahomaRentals.Server.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace OklahomaRentals.Server.Services.Implementations
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRequestRepository _repository;
        private readonly INotificationService _notificationService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IConfiguration _config;

        public MaintenanceService(
            IMaintenanceRequestRepository repository,
            INotificationService notificationService,
            ITenantRepository tenantRepository,
            IUserRepository userRepository,
            IPropertyRepository propertyRepository,
            IAdminRepository adminRepository,
            IConfiguration config)
        {
            _repository = repository;
            _notificationService = notificationService;
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _propertyRepository = propertyRepository;
            _adminRepository = adminRepository;
            _config = config;
        }

        public async Task<IEnumerable<MaintenanceRequestResponse>> GetAllAsync()
        {
            var requests = await _repository.GetAllAsync();

            return requests.Select(r => new MaintenanceRequestResponse
            {
                Id = r.Id,
                Title = r.Title,
                PropertyId = r.PropertyId,
                Description = r.Description,
                Status = r.Status,
                DateRequested = r.DateRequested,
                DateResolved = r.DateResolved
            });
        }

        public async Task<MaintenanceRequestResponse?> GetByIdAsync(int id)
        {
            var request = await _repository.GetByIdAsync(id);

            if (request == null)
                return null;

            return new MaintenanceRequestResponse
            {
                Id = request.Id,
                Title = request.Title,
                PropertyId = request.PropertyId,
                Description = request.Description,
                Status = request.Status,
                DateRequested = request.DateRequested,
                DateResolved = request.DateResolved
            };
        }

        public async Task<MaintenanceRequestResponse> CreateAsync(MaintenanceRequestCreateRequest request)
        {
            var entity = new Data.Entities.MaintenanceRequest
            {
                TenantId = request.TenantId,
                PropertyId = request.PropertyId,
                Title = request.Title,
                Description = request.Description,
                Status = "Pending", // pending by default
                DateRequested = DateTime.UtcNow,
                DateResolved = null
            };

            var created = await _repository.CreateAsync(entity);

            var tenant = await _tenantRepository.GetByIdAsync(created.TenantId);
            var tenantUser = tenant != null ? await _userRepository.GetByIdAsync(tenant.UserId) : null;
            var property = await _propertyRepository.GetByIdAsync(created.PropertyId);
            var admin = property != null ? await _adminRepository.GetByIdAsync(property.AdminId) : null;
            var landlordUser = admin != null ? await _userRepository.GetByIdAsync(admin.UserId) : null;
            var tenantName = tenantUser?.FullName ?? tenant?.Name ?? $"Tenant {created.TenantId}";
            var propertyAddress = property != null ? $"{property.Name} ({property.Address})" : $"Property {created.PropertyId}";

            var landlordPhone = _config["Landlord:PhoneNumber"];

            if (landlordUser != null && !string.IsNullOrWhiteSpace(landlordPhone))
            {
                await _notificationService.SendMaintenanceRequestSubmittedAsync(
                    landlordUserId: landlordUser.Email,
                    landlordEmail: landlordUser.Email,
                    landlordPhoneNumber: landlordPhone,
                    tenantName: tenantName,
                    propertyAddress: propertyAddress,
                    requestTitle: created.Title,
                    requestPriority: request.RequestPriority,
                    requestDescription: created.Description
                );
            }

            return new MaintenanceRequestResponse
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                Status = created.Status,
                DateRequested = created.DateRequested,
                DateResolved = created.DateResolved
            };
        }

        private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "Pending",
            "Completed",
            "Cancelled"
        };

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            if (!AllowedStatuses.Contains(status))
                throw new ArgumentException($"Invalid Status: {status}");

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return false;

            entity.Status = status;

            if (string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase))
                entity.DateResolved = DateTime.UtcNow;
            else
                entity.DateResolved = null;

            await _repository.UpdateAsync(entity);

            var tenant = await _tenantRepository.GetByIdAsync(entity.TenantId);
            var tenantUser = tenant != null ? await _userRepository.GetByIdAsync(tenant.UserId) : null;
            var property = await _propertyRepository.GetByIdAsync(entity.PropertyId);

            if (tenant != null && tenantUser != null && property != null)
            {
                await _notificationService.SendMaintenanceRequestStatusUpdateAsync(
                    tenantUserId: tenantUser.Email,
                    tenantEmail: tenantUser.Email,
                    tenantPhoneNumber: tenant.PhoneNumber,
                    propertyAddress: $"{property.Name} ({property.Address})",
                    requestTitle: entity.Title,
                    requestStatus: entity.Status,
                    requestDescription: entity.Description
                );
            }

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MaintenanceRequestResponse>> GetMaintenanceRequestsByTenantAsync(int tenantId)
        {
            var requests = await _repository.GetAllAsync();

            return requests
                .Where(r => r.TenantId == tenantId)
                .Select(r => new MaintenanceRequestResponse
                {
                    Id = r.Id,
                    Title = r.Title,
                    PropertyId = r.PropertyId,
                    Description = r.Description,
                    Status = r.Status,
                    DateRequested = r.DateRequested,
                    DateResolved = r.DateResolved
                });
        }

        public async Task<IEnumerable<MaintenanceRequestResponse>> GetMaintenanceRequestsByPropertyAsync(int propertyId)
        {
            var requests = await _repository.GetAllAsync();

            return requests
                .Where(r => r.PropertyId == propertyId)
                .Select(r => new MaintenanceRequestResponse
                {
                    Id = r.Id,
                    Title = r.Title,
                    PropertyId = r.PropertyId,
                    Description = r.Description,
                    Status = r.Status,
                    DateRequested = r.DateRequested,
                    DateResolved = r.DateResolved
                });
        }
    }
}