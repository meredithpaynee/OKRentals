using System.Collections.Generic;
using System.Threading.Tasks;
using OklahomaRentals.Server.API.Response;

namespace OklahomaRentals.Server.Services.Interfaces
{
    public interface ITenantService
    {
        Task<IEnumerable<TenantResponse>> GetAllAsync();
        Task<TenantResponse?> GetByIdAsync(int id);
        Task<TenantResponse> CreateAsync(TenantResponse tenant);
        Task<TenantResponse?> UpdateAsync(int id, TenantResponse tenant);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<TenantResponse>> GetTenantsByPropertyAsync(int propertyId);
        Task<TenantResponse?> AssignTenantToPropertyAsync(int tenantId, int propertyId);
        
        // Tenant self-service methods
        Task<LeaseInformationResponse?> GetLeaseInformationAsync(int tenantId);
        Task<TenantAccountSummaryResponse?> GetAccountSummaryAsync(int tenantId);
        Task<IEnumerable<UpcomingPaymentResponse>> GetUpcomingPaymentsAsync(int tenantId);
    }
}