using OklahomaRentals.Server.Data.Entities;

namespace OklahomaRentals.Server.Repositories.Interfaces
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property>> GetAllAsync();
        Task<Property?> GetByIdAsync(int id);
        Task<Property> CreateAsync(Property property);
        Task<Property> UpdateAsync(Property property);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Records that the database would cascade-delete along with the property,
        // as (label, count) pairs with a non-zero count.
        Task<IReadOnlyList<(string Label, int Count)>> GetDependentRecordsAsync(int id);
    }
}