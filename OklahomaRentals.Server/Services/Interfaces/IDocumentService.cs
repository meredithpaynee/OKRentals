using System.Collections.Generic;
using System.Threading.Tasks;
using OklahomaRentals.Server.API.Response;

namespace OklahomaRentals.Server.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentResponse>> GetAllAsync();
        Task<DocumentResponse?> GetByIdAsync(int id);
        Task<DocumentResponse> CreateAsync(DocumentResponse document);
        Task<DocumentResponse?> UpdateAsync(int id, DocumentResponse document);
        Task<bool> DeleteAsync(int id);
    }
}