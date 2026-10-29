using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace OklahomaRentals.Server.Services.Implementations
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _repository;
        private readonly INotificationService _notificationService;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public ApplicationService(
            IApplicationRepository repository,
            INotificationService notificationService,
            IPropertyRepository propertyRepository,
            IAdminRepository adminRepository,
            IUserRepository userRepository,
            IConfiguration config)
        {
            _repository = repository;
            _notificationService = notificationService;
            _propertyRepository = propertyRepository;
            _adminRepository = adminRepository;
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<IEnumerable<ApplicationResponse>> GetAllAsync()
        {
            var applications = await _repository.GetAllAsync();

            return applications.Select(a => new ApplicationResponse
            {
                Id = a.Id,
                PropertyId = a.PropertyId,
                Status = a.Status,
                Name = a.Name,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                BirthDate = a.BirthDate,
                MoveInDate = a.MoveInDate,
                DateSubmitted = a.DateSubmitted,
                FileName = a.FileName
            });
        }

        public async Task<ApplicationResponse?> GetByIdAsync(int id)
        {
            var application = await _repository.GetByIdAsync(id);

            if (application == null)
                return null;

            return new ApplicationResponse
            {
                Id = application.Id,
                PropertyId = application.PropertyId,
                Status = application.Status,
                Name = application.Name,
                Email = application.Email,
                PhoneNumber = application.PhoneNumber,
                BirthDate = application.BirthDate,
                MoveInDate = application.MoveInDate,
                DateSubmitted = application.DateSubmitted,
                FileName = application.FileName
            };
        }

        public async Task<ApplicationResponse> CreateAsync(ApplicationResponse applicationRequest)
        {
            var entity = new Data.Entities.Application
            {
                PropertyId = applicationRequest.PropertyId,
                Status = applicationRequest.Status,
                Name = applicationRequest.Name,
                Email = applicationRequest.Email,
                PhoneNumber = applicationRequest.PhoneNumber,
                BirthDate = applicationRequest.BirthDate,
                MoveInDate = applicationRequest.MoveInDate,
                DateSubmitted = DateTime.UtcNow,
                FileName = applicationRequest.FileName
            };

            var created = await _repository.CreateAsync(entity);

            var property = await _propertyRepository.GetByIdAsync(created.PropertyId);
            var admin = property != null ? await _adminRepository.GetByIdAsync(property.AdminId) : null;
            var landlordUser = admin != null ? await _userRepository.GetByIdAsync(admin.UserId) : null;

            var landlordPhone = _config["Landlord:PhoneNumber"];
            var propertyAddress = property != null ? $"{property.Name} ({property.Address})" : $"Property {created.PropertyId}";

            if (landlordUser != null && !string.IsNullOrWhiteSpace(landlordPhone))
            {
                await _notificationService.SendApplicationSubmittedAsync(
                    landlordUserId: landlordUser.Email,
                    landlordEmail: landlordUser.Email,
                    landlordPhoneNumber: landlordPhone,
                    propertyAddress: propertyAddress,
                    applicantName: created.Name,
                    applicantEmail: created.Email,
                    applicantPhone: created.PhoneNumber,
                    moveInDate: created.MoveInDate.ToString("yyyy-MM-dd")
                );
            }

            return new ApplicationResponse
            {
                Id = created.Id,
                PropertyId = created.PropertyId,
                Status = "Pending", // pending by default
                Name = created.Name,
                Email = created.Email,
                PhoneNumber = created.PhoneNumber,
                BirthDate = created.BirthDate,
                MoveInDate = created.MoveInDate,
                DateSubmitted = created.DateSubmitted,
                FileName = created.FileName
            };
        }

        public async Task<ApplicationResponse?> UpdateAsync(int id, ApplicationResponse applicationRequest)
        {
            var app = await _repository.GetByIdAsync(id);
            if (app == null)
                return null;

            app.PropertyId = applicationRequest.PropertyId;
            app.Status = applicationRequest.Status;
            app.Name = applicationRequest.Name;
            app.Email = applicationRequest.Email;
            app.PhoneNumber = applicationRequest.PhoneNumber;
            app.BirthDate = applicationRequest.BirthDate;
            app.MoveInDate = applicationRequest.MoveInDate;
            app.FileName = applicationRequest.FileName;

            var updated = await _repository.UpdateAsync(app);

            return new ApplicationResponse
            {
                Id = updated.Id,
                PropertyId = updated.PropertyId,
                Status = updated.Status,
                Name = updated.Name,
                Email = updated.Email,
                PhoneNumber = updated.PhoneNumber,
                BirthDate = updated.BirthDate,
                MoveInDate = updated.MoveInDate,
                DateSubmitted = updated.DateSubmitted,
                FileName = updated.FileName
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}