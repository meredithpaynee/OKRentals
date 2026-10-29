using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.Repositories.Interfaces;

namespace OklahomaRentals.Server.Services.Implementations
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _repository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;

        public TenantService(
            ITenantRepository repository,
            IPropertyRepository propertyRepository,
            IPaymentRepository paymentRepository,
            IAdminRepository adminRepository,
            IUserRepository userRepository)
        {
            _repository = repository;
            _propertyRepository = propertyRepository;
            _paymentRepository = paymentRepository;
            _adminRepository = adminRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<TenantResponse>> GetAllAsync()
        {
            var tenants = await _repository.GetAllAsync();

            return tenants.Select(t => new TenantResponse
            {
                Id = t.Id,
                PropertyId = t.PropertyId,
                Name = t.User?.FullName ?? string.Empty,
                Email = t.User?.Email ?? string.Empty,
                PhoneNumber = t.PhoneNumber,
                BirthDate = t.BirthDate
            });
        }

        public async Task<TenantResponse?> GetByIdAsync(int id)
        {
            var tenant = await _repository.GetByIdAsync(id);

            if (tenant == null)
                return null;

            return new TenantResponse
            {
                Id = tenant.Id,
                PropertyId = tenant.PropertyId,
                Name = tenant.User?.FullName ?? string.Empty,
                Email = tenant.User?.Email ?? string.Empty,
                PhoneNumber = tenant.PhoneNumber,
                BirthDate = tenant.BirthDate
            };
        }

        public async Task<TenantResponse> CreateAsync(TenantResponse tenantRequest)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(tenantRequest.Email))
                    throw new ArgumentException("Email is required");
                if (string.IsNullOrWhiteSpace(tenantRequest.Name))
                    throw new ArgumentException("Name is required");
                if (tenantRequest.PropertyId <= 0)
                    throw new ArgumentException("Valid PropertyId is required");

                // Create a new User record for the tenant
                var newUser = new Data.Entities.User
                {
                    Email = tenantRequest.Email,
                    FullName = tenantRequest.Name,
                    PasswordHash = "temp_hash_to_be_replaced",
                    Role = "Tenant"
                };

                var createdUser = await _userRepository.CreateAsync(newUser);

                if (createdUser == null)
                    throw new Exception("Failed to create user record");

                // Create Tenant with the new UserId
                var entity = new Data.Entities.Tenant
                {
                    PropertyId = tenantRequest.PropertyId,
                    PhoneNumber = tenantRequest.PhoneNumber,
                    BirthDate = tenantRequest.BirthDate,
                    LeaseStartDate = DateTime.UtcNow,
                    UserId = createdUser.Id,
                    Name = tenantRequest.Name  // Also store in Tenant entity
                };

                var created = await _repository.CreateAsync(entity);

                if (created == null)
                    throw new Exception("Failed to create tenant record");

                // Manually build response with known values
                return new TenantResponse
                {
                    Id = created.Id,
                    PropertyId = created.PropertyId,
                    Name = created.Name ?? createdUser.FullName,  // Use Tenant name first, fallback to User
                    Email = createdUser.Email,
                    PhoneNumber = created.PhoneNumber,
                    BirthDate = created.BirthDate
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tenant: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<TenantResponse?> UpdateAsync(int id, TenantResponse tenantRequest)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return null;
            }

            existing.PropertyId = tenantRequest.PropertyId;
            existing.PhoneNumber = tenantRequest.PhoneNumber;
            existing.BirthDate = tenantRequest.BirthDate;
            // LeaseStartDate, LeaseEndDate, UserId left as-is for now

            await _repository.UpdateAsync(existing);

            return new TenantResponse
            {
                Id = existing.Id,
                PropertyId = existing.PropertyId,
                Name = tenantRequest.Name,
                Email = tenantRequest.Email,
                PhoneNumber = existing.PhoneNumber,
                BirthDate = existing.BirthDate
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TenantResponse>> GetTenantsByPropertyAsync(int propertyId)
        {
            var tenants = await _repository.GetAllAsync();

            return tenants
                .Where(t => t.PropertyId == propertyId)
                .Select(t => new TenantResponse
                {
                    Id = t.Id,
                    PropertyId = t.PropertyId,
                    Name = t.User?.FullName ?? string.Empty,
                    Email = t.User?.Email ?? string.Empty,
                    PhoneNumber = t.PhoneNumber,
                    BirthDate = t.BirthDate
                });
        }

        public async Task<TenantResponse?> AssignTenantToPropertyAsync(int tenantId, int propertyId)
        {
            var tenant = await _repository.GetByIdAsync(tenantId);
            if (tenant == null)
                return null;

            tenant.PropertyId = propertyId;
            await _repository.UpdateAsync(tenant);

            return new TenantResponse
            {
                Id = tenant.Id,
                PropertyId = tenant.PropertyId,
                Name = tenant.User?.FullName ?? string.Empty,
                Email = tenant.User?.Email ?? string.Empty,
                PhoneNumber = tenant.PhoneNumber,
                BirthDate = tenant.BirthDate
            };
        }

        public async Task<LeaseInformationResponse?> GetLeaseInformationAsync(int tenantId)
        {
            var tenant = await _repository.GetByIdAsync(tenantId);
            if (tenant == null)
                return null;

            var property = await _propertyRepository.GetByIdAsync(tenant.PropertyId);
            if (property == null)
                return null;

            var admin = await _adminRepository.GetByIdAsync(property.AdminId);
            var landlordUser = admin != null ? await _userRepository.GetByIdAsync(admin.UserId) : null;

            var daysRemaining = tenant.LeaseEndDate.HasValue 
                ? (int)(tenant.LeaseEndDate.Value - DateTime.UtcNow).TotalDays 
                : 0;
            var isActiveLease = !tenant.LeaseEndDate.HasValue || tenant.LeaseEndDate > DateTime.UtcNow;

            return new LeaseInformationResponse
            {
                TenantId = tenant.Id,
                PropertyId = property.Id,
                PropertyName = property.Name,
                PropertyAddress = property.Address,
                LeaseStartDate = tenant.LeaseStartDate,
                LeaseEndDate = tenant.LeaseEndDate,
                IsActiveLease = isActiveLease,
                DaysRemainingInLease = daysRemaining,
                MonthlyRentAmount = property.Price,
                LandlordName = landlordUser?.FullName ?? string.Empty,
                LandlordEmail = landlordUser?.Email ?? string.Empty,
                LandlordPhoneNumber = string.Empty // Phone number stored in appsettings.json or Admin profile
            };
        }

        public async Task<TenantAccountSummaryResponse?> GetAccountSummaryAsync(int tenantId)
        {
            var tenant = await _repository.GetByIdAsync(tenantId);
            if (tenant == null)
                return null;

            var payments = await _paymentRepository.GetAllAsync();
            var tenantPayments = payments.Where(p => p.TenantId == tenantId).ToList();

            var totalDue = tenantPayments.Sum(p => p.AmountDue);
            var totalPaid = tenantPayments.Sum(p => p.AmountPaid);
            var currentBalance = totalDue - totalPaid;
            
            var paidPayments = tenantPayments.Count(p => p.Status == "Paid");
            var outstandingPayments = tenantPayments.Count(p => p.AmountDue > p.AmountPaid);
            var overduePayments = tenantPayments.Count(p => p.DueDate < DateTime.UtcNow && p.AmountDue > p.AmountPaid);
            
            var nextPayment = tenantPayments
                .Where(p => p.AmountDue > p.AmountPaid)
                .OrderBy(p => p.DueDate)
                .FirstOrDefault();

            return new TenantAccountSummaryResponse
            {
                TenantId = tenant.Id,
                TenantName = tenant.User?.FullName ?? tenant.Name,
                TotalDue = totalDue,
                TotalPaid = totalPaid,
                CurrentBalance = currentBalance,
                TotalPayments = tenantPayments.Count,
                PaidPayments = paidPayments,
                OutstandingPayments = outstandingPayments,
                OverduePayments = overduePayments,
                NextPaymentDueDate = nextPayment?.DueDate,
                NextPaymentAmount = nextPayment?.AmountDue,
                HasOutstandingBalance = currentBalance > 0,
                HasOverduePayments = overduePayments > 0
            };
        }

        public async Task<IEnumerable<UpcomingPaymentResponse>> GetUpcomingPaymentsAsync(int tenantId)
        {
            var payments = await _paymentRepository.GetAllAsync();
            var tenantPayments = payments
                .Where(p => p.TenantId == tenantId && p.AmountDue > p.AmountPaid)
                .OrderBy(p => p.DueDate)
                .ToList();

            return tenantPayments.Select(p => new UpcomingPaymentResponse
            {
                PaymentId = p.Id,
                DueDate = p.DueDate,
                Amount = p.AmountDue - p.AmountPaid,
                Status = p.Status,
                IsOverdue = p.DueDate < DateTime.UtcNow,
                DaysUntilDue = (int)(p.DueDate - DateTime.UtcNow).TotalDays
            });
        }
    }
}