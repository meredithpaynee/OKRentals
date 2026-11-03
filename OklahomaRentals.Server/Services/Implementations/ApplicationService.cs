using System.Globalization;
using OklahomaRentals.Server.API.Requests;
using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.Data.Entities;
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
            var propertyNames = (await _propertyRepository.GetAllAsync()).ToDictionary(p => p.Id, p => p.Name);

            return applications
                .OrderByDescending(a => a.DateSubmitted)
                .Select(a => ToResponse(a, propertyNames.GetValueOrDefault(a.PropertyId)));
        }

        public async Task<ApplicationResponse?> GetByIdAsync(int id)
        {
            var application = await _repository.GetByIdAsync(id);
            if (application == null)
                return null;

            var property = await _propertyRepository.GetByIdAsync(application.PropertyId);
            return ToResponse(application, property?.Name);
        }

        private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "Pending",
            "Approved",
            "Declined"
        };

        public async Task<ApplicationResponse> SubmitAsync(RentalApplicationRequest request)
        {
            var property = (await _propertyRepository.GetAllAsync())
                .FirstOrDefault(p => string.Equals(p.Name, request.Property, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArgumentException($"Unknown property \"{request.Property}\".", nameof(request.Property));

            var entity = new Data.Entities.Application
            {
                PropertyId = property.Id,
                Status = "Pending",
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.Phone,
                BirthDate = RequiredDate(request.DateOfBirth, nameof(request.DateOfBirth)),
                MoveInDate = RequiredDate(request.LeaseStart, nameof(request.LeaseStart)),
                DateSubmitted = DateTime.UtcNow,
                DriversLicense = request.DriversLicense,
                College = request.College,
                Major = request.Major,
                ExpectedGraduation = OptionalDate(request.ExpectedGraduation),
                LeaseTerm = request.LeaseTerm,
                CurrentResidence = ToRecord(request.CurrentResidence),
                PreviousResidence = ToRecord(request.PreviousResidence),
                Evicted = YesNo(request.Evicted),
                BrokenLease = YesNo(request.BrokenLease),
                Bankruptcy = YesNo(request.Bankruptcy),
                Convicted = YesNo(request.Convicted),
                Explanation = request.Explanation,
            };

            var created = await _repository.CreateAsync(entity);
            return ToResponse(created, property.Name);
        }

        public async Task<ApplicationResponse?> UpdateStatusAsync(int id, string status)
        {
            if (!AllowedStatuses.Contains(status))
                throw new ArgumentException($"Invalid status: {status}", nameof(status));

            var application = await _repository.GetByIdAsync(id);
            if (application == null)
                return null;

            application.Status = AllowedStatuses.First(s => s.Equals(status, StringComparison.OrdinalIgnoreCase));
            await _repository.UpdateAsync(application);

            var property = await _propertyRepository.GetByIdAsync(application.PropertyId);
            return ToResponse(application, property?.Name);
        }

        private static DateTime RequiredDate(string value, string field) =>
            OptionalDate(value) ?? throw new ArgumentException($"{field} must be a date (yyyy-mm-dd).", field);

        // The form sends dates as yyyy-mm-dd.
        private static DateTime? OptionalDate(string? value) =>
            DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                ? date
                : null;

        private static bool? YesNo(string? answer) => answer?.ToLowerInvariant() switch
        {
            "yes" => true,
            "no" => false,
            _ => null,
        };

        private static ResidenceRecord? ToRecord(ResidenceHistory? r)
        {
            if (r == null || (string.IsNullOrWhiteSpace(r.Address) && string.IsNullOrWhiteSpace(r.Landlord)))
                return null;

            return new ResidenceRecord
            {
                Address = r.Address,
                Landlord = r.Landlord,
                LandlordContact = r.LandlordContact,
                From = OptionalDate(r.From),
                To = OptionalDate(r.To),
                ReasonForLeaving = r.ReasonForLeaving,
            };
        }

        private static ResidenceResponse? ToResponse(ResidenceRecord? r) => r == null ? null : new ResidenceResponse
        {
            Address = r.Address,
            Landlord = r.Landlord,
            LandlordContact = r.LandlordContact,
            From = r.From,
            To = r.To,
            ReasonForLeaving = r.ReasonForLeaving,
        };

        private static ApplicationResponse ToResponse(Data.Entities.Application a, string? propertyName) => new()
        {
            Id = a.Id,
            PropertyId = a.PropertyId,
            PropertyName = propertyName,
            Status = a.Status,
            Name = a.Name,
            Email = a.Email,
            PhoneNumber = a.PhoneNumber,
            BirthDate = a.BirthDate,
            MoveInDate = a.MoveInDate,
            DateSubmitted = a.DateSubmitted,
            FileName = a.FileName,
            DriversLicense = a.DriversLicense,
            College = a.College,
            Major = a.Major,
            ExpectedGraduation = a.ExpectedGraduation,
            LeaseTerm = a.LeaseTerm,
            CurrentResidence = ToResponse(a.CurrentResidence),
            PreviousResidence = ToResponse(a.PreviousResidence),
            Evicted = a.Evicted,
            BrokenLease = a.BrokenLease,
            Bankruptcy = a.Bankruptcy,
            Convicted = a.Convicted,
            Explanation = a.Explanation,
        };

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