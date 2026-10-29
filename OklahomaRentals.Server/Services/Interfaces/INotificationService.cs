using System.Threading.Tasks;

namespace OklahomaRentals.Server.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendMaintenanceRequestSubmittedAsync(
            string landlordUserId,
            string landlordEmail,
            string landlordPhoneNumber,
            string tenantName,
            string propertyAddress,
            string requestTitle,
            string requestPriority,
            string requestDescription
        );

        Task SendMaintenanceRequestStatusUpdateAsync(
            string tenantUserId,
            string tenantEmail,
            string tenantPhoneNumber,
            string propertyAddress,
            string requestTitle,
            string requestStatus,
            string requestDescription
        );

        Task SendApplicationSubmittedAsync(
            string landlordUserId,
            string landlordEmail,
            string landlordPhoneNumber,
            string propertyAddress,
            string applicantName,
            string applicantEmail,
            string applicantPhone,
            string moveInDate
        );

        Task SendPaymentSubmittedAsync(
            string landlordUserId,
            string landlordEmail,
            string landlordPhoneNumber,
            string tenantName,
            string propertyAddress,
            string amountPaid,
            string paymentDate
        );

        Task SendTenantDocumentUploadedAsync(
            string landlordUserId,
            string landlordEmail,
            string landlordPhoneNumber,
            string tenantName,
            string propertyAddress,
            string documentType,
            string fileName
        );
    }
}
