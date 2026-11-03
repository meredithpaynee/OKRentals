using OklahomaRentals.Server.API.Requests;

namespace OklahomaRentals.Server.Services.Interfaces
{
    public interface IApplicationEmailService
    {
        // Emails a submitted rental application to the office inbox.
        Task SendAsync(RentalApplicationRequest application);
    }
}
