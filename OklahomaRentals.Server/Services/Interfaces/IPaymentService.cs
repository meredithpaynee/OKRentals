using System.Collections.Generic;
using System.Threading.Tasks;
using OklahomaRentals.Server.API.Response;

namespace OklahomaRentals.Server.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentResponse>> GetAllAsync();
        Task<PaymentResponse?> GetByIdAsync(int id);
        Task<PaymentResponse> CreateAsync(PaymentResponse payment);
        Task<PaymentResponse?> UpdateAsync(int id, PaymentResponse payment);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<PaymentResponse>> GetPaymentsByTenantAsync(int tenantId);
        Task<IEnumerable<PaymentResponse>> GetPaymentsByPropertyAsync(int propertyId);
        
        // Landlord-specific methods
        Task<IEnumerable<PaymentWithTenantResponse>> GetIncomingPaymentsAsync(int propertyId);
        Task<IEnumerable<TenantOutstandingBalanceResponse>> GetTenantsWithOutstandingBalancesAsync(int propertyId);
    }
}