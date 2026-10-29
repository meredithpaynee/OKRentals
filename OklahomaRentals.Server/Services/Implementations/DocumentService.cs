using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace OklahomaRentals.Server.Services.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _repository;
        private readonly INotificationService _notificationService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IConfiguration _config;

        public DocumentService(
            IDocumentRepository repository,
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

        public async Task<IEnumerable<DocumentResponse>> GetAllAsync()
        {
            var documents = await _repository.GetAllAsync();

            return documents.Select(d => new DocumentResponse
            {
                Id = d.Id,
                PropertyId = d.PropertyId,
                TenantId = d.TenantId,
                Type = d.Type,
                FileName = d.FileName,
                DateSubmitted = d.DateSubmitted
            });
        }

        public async Task<DocumentResponse?> GetByIdAsync(int id)
        {
            var document = await _repository.GetByIdAsync(id);

            if (document == null)
                return null;

            return new DocumentResponse
            {
                Id = document.Id,
                PropertyId = document.PropertyId,
                TenantId = document.TenantId,
                Type = document.Type,
                FileName = document.FileName,
                DateSubmitted = document.DateSubmitted
            };
        }

        public async Task<DocumentResponse> CreateAsync(DocumentResponse documentRequest)
        {
            var entity = new Data.Entities.Document
            {
                PropertyId = documentRequest.PropertyId,
                TenantId = documentRequest.TenantId,
                Type = documentRequest.Type,
                FileName = documentRequest.FileName,
                DateSubmitted = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(entity);
            
            var tenant = await _tenantRepository.GetByIdAsync(created.TenantId);
            var tenantUser = tenant != null ? await _userRepository.GetByIdAsync(tenant.UserId) : null;
            var property = await _propertyRepository.GetByIdAsync(created.PropertyId);
            var admin = property != null ? await _adminRepository.GetByIdAsync(property.AdminId) : null;
            var landlordUser = admin != null ? await _userRepository.GetByIdAsync(admin.UserId) : null;
            var landlordPhone = _config["Landlord:PhoneNumber"];
            var tenantName = tenantUser?.FullName ?? tenant?.Name ?? $"Tenant {created.TenantId}";
            var propertyAddress = property != null ? $"{property.Name} ({property.Address})" : $"Property {created.PropertyId}";

            if (landlordUser != null && !string.IsNullOrWhiteSpace(landlordPhone))
            {
                await _notificationService.SendTenantDocumentUploadedAsync(
                    landlordUserId: landlordUser.Email,
                    landlordEmail: landlordUser.Email,
                    landlordPhoneNumber: landlordPhone,
                    tenantName: tenantName,
                    propertyAddress: propertyAddress,
                    documentType: created.Type,
                    fileName: created.FileName
                );
            }

            return new DocumentResponse
            {
                Id = created.Id,
                PropertyId = created.PropertyId,
                TenantId = created.TenantId,
                Type = created.Type,
                FileName = created.FileName,
                DateSubmitted = created.DateSubmitted
            };
        }

        public async Task<DocumentResponse?> UpdateAsync(int id, DocumentResponse documentRequest)
        {
            var doc = await _repository.GetByIdAsync(id);
            if (doc == null)
                return null;

            doc.PropertyId = documentRequest.PropertyId;
            doc.TenantId = documentRequest.TenantId;
            doc.Type = documentRequest.Type;
            doc.FileName = documentRequest.FileName;

            var updated = await _repository.UpdateAsync(doc);

            return new DocumentResponse
            {
                Id = updated.Id,
                PropertyId = updated.PropertyId,
                TenantId = updated.TenantId,
                Type = updated.Type,
                FileName = updated.FileName,
                DateSubmitted = updated.DateSubmitted
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}