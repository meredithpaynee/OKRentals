using OklahomaRentals.Server.API.Response;
using OklahomaRentals.Server.Services.Interfaces;
using OklahomaRentals.Server.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace OklahomaRentals.Server.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly INotificationService _notificationService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IConfiguration _config;

        public PaymentService(
            IPaymentRepository repository,
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

        public async Task<IEnumerable<PaymentResponse>> GetAllAsync()
        {
            var payments = await _repository.GetAllAsync();

            return payments.Select(p => new PaymentResponse
            {
                Id = p.Id,
                TenantId = p.TenantId,
                PropertyId = p.PropertyId,
                DueDate = p.DueDate,
                PaymentDate = p.PaymentDate,
                AmountDue = p.AmountDue,
                AmountPaid = p.AmountPaid,
                Status = p.Status
            });
        }

        public async Task<PaymentResponse?> GetByIdAsync(int id)
        {
            var payment = await _repository.GetByIdAsync(id);

            if (payment == null)
                return null;

            return new PaymentResponse
            {
                Id = payment.Id,
                TenantId = payment.TenantId,
                PropertyId = payment.PropertyId,
                DueDate = payment.DueDate,
                PaymentDate = payment.PaymentDate,
                AmountDue = payment.AmountDue,
                AmountPaid = payment.AmountPaid,
                Status = payment.Status
            };
        }

        public async Task<PaymentResponse> CreateAsync(PaymentResponse paymentRequest)
        {
            var entity = new Data.Entities.Payment
            {
                TenantId = paymentRequest.TenantId,
                PropertyId = paymentRequest.PropertyId,
                DueDate = paymentRequest.DueDate,
                PaymentDate = paymentRequest.PaymentDate,
                AmountDue = paymentRequest.AmountDue,
                AmountPaid = paymentRequest.AmountPaid,
                Status = paymentRequest.Status
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
            var amountPaid = created.AmountPaid.ToString("0.00");
            var paymentDate = (created.PaymentDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd");

            if (landlordUser != null && !string.IsNullOrWhiteSpace(landlordPhone))
            {
                await _notificationService.SendPaymentSubmittedAsync(
                    landlordUserId: landlordUser.Email,
                    landlordEmail: landlordUser.Email,
                    landlordPhoneNumber: landlordPhone,
                    tenantName: tenantName,
                    propertyAddress: propertyAddress,
                    amountPaid: amountPaid,
                    paymentDate: paymentDate
                );
            }

            return new PaymentResponse
            {
                Id = created.Id,
                TenantId = created.TenantId,
                PropertyId = created.PropertyId,
                DueDate = created.DueDate,
                PaymentDate = created.PaymentDate,
                AmountDue = created.AmountDue,
                AmountPaid = created.AmountPaid,
                Status = created.Status
            };
        }

        public async Task<PaymentResponse?> UpdateAsync(int id, PaymentResponse paymentRequest)
        {
            var payment = await _repository.GetByIdAsync(id);
            if (payment == null)
                return null;

            payment.TenantId = paymentRequest.TenantId;
            payment.PropertyId = paymentRequest.PropertyId;
            payment.DueDate = paymentRequest.DueDate;
            payment.PaymentDate = paymentRequest.PaymentDate;
            payment.AmountDue = paymentRequest.AmountDue;
            payment.AmountPaid = paymentRequest.AmountPaid;
            payment.Status = paymentRequest.Status;

            var updated = await _repository.UpdateAsync(payment);

            return new PaymentResponse
            {
                Id = updated.Id,
                TenantId = updated.TenantId,
                PropertyId = updated.PropertyId,
                DueDate = updated.DueDate,
                PaymentDate = updated.PaymentDate,
                AmountDue = updated.AmountDue,
                AmountPaid = updated.AmountPaid,
                Status = updated.Status
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<PaymentResponse>> GetPaymentsByTenantAsync(int tenantId)
        {
            var payments = await _repository.GetAllAsync();

            return payments
                .Where(p => p.TenantId == tenantId)
                .Select(p => new PaymentResponse
                {
                    Id = p.Id,
                    TenantId = p.TenantId,
                    PropertyId = p.PropertyId,
                    DueDate = p.DueDate,
                    PaymentDate = p.PaymentDate,
                    AmountDue = p.AmountDue,
                    AmountPaid = p.AmountPaid,
                    Status = p.Status
                });
        }

        public async Task<IEnumerable<PaymentResponse>> GetPaymentsByPropertyAsync(int propertyId)
        {
            var payments = await _repository.GetAllAsync();

            return payments
                .Where(p => p.PropertyId == propertyId)
                .Select(p => new PaymentResponse
                {
                    Id = p.Id,
                    TenantId = p.TenantId,
                    PropertyId = p.PropertyId,
                    DueDate = p.DueDate,
                    PaymentDate = p.PaymentDate,
                    AmountDue = p.AmountDue,
                    AmountPaid = p.AmountPaid,
                    Status = p.Status
                });
        }

        public async Task<IEnumerable<PaymentWithTenantResponse>> GetIncomingPaymentsAsync(int propertyId)
        {
            var payments = await _repository.GetAllAsync();
            var tenants = await _tenantRepository.GetAllAsync();

            return payments
                .Where(p => p.PropertyId == propertyId)
                .Join(tenants, p => p.TenantId, t => t.Id, (p, t) => new { Payment = p, Tenant = t })
                .Select(pt => new PaymentWithTenantResponse
                {
                    Id = pt.Payment.Id,
                    TenantId = pt.Payment.TenantId,
                    PropertyId = pt.Payment.PropertyId,
                    TenantName = pt.Tenant.User?.FullName ?? string.Empty,
                    TenantEmail = pt.Tenant.User?.Email ?? string.Empty,
                    TenantPhoneNumber = pt.Tenant.PhoneNumber,
                    DueDate = pt.Payment.DueDate,
                    PaymentDate = pt.Payment.PaymentDate,
                    AmountDue = pt.Payment.AmountDue,
                    AmountPaid = pt.Payment.AmountPaid,
                    OutstandingBalance = pt.Payment.AmountDue - pt.Payment.AmountPaid,
                    Status = pt.Payment.Status
                });
        }

        public async Task<IEnumerable<TenantOutstandingBalanceResponse>> GetTenantsWithOutstandingBalancesAsync(int propertyId)
        {
            var payments = await _repository.GetAllAsync();
            var tenants = await _tenantRepository.GetAllAsync();

            var tenantsWithBalances = payments
                .Where(p => p.PropertyId == propertyId)
                .GroupBy(p => p.TenantId)
                .Select(g => new
                {
                    TenantId = g.Key,
                    TotalOutstandingBalance = g.Sum(p => p.AmountDue - p.AmountPaid),
                    OverduePayments = g.Where(p => p.DueDate < DateTime.UtcNow && (p.AmountDue - p.AmountPaid) > 0),
                    AllPayments = g
                })
                .Where(t => t.TotalOutstandingBalance > 0)
                .Join(tenants, tb => tb.TenantId, t => t.Id, (tb, t) => new { TenantBalance = tb, Tenant = t })
                .Select(tt => new TenantOutstandingBalanceResponse
                {
                    TenantId = tt.TenantBalance.TenantId,
                    PropertyId = propertyId,
                    TenantName = tt.Tenant.User?.FullName ?? string.Empty,
                    TenantEmail = tt.Tenant.User?.Email ?? string.Empty,
                    TenantPhoneNumber = tt.Tenant.PhoneNumber,
                    TotalOutstandingBalance = tt.TenantBalance.TotalOutstandingBalance,
                    OverduePaymentCount = tt.TenantBalance.OverduePayments.Count(),
                    EarliestOverdueDate = tt.TenantBalance.OverduePayments.Any() 
                        ? tt.TenantBalance.OverduePayments.Min(p => p.DueDate) 
                        : null
                });

            return tenantsWithBalances;
        }
    }
}