using Microsoft.EntityFrameworkCore;
using OklahomaRentals.Server.Data;
using OklahomaRentals.Server.Data.Entities;
using OklahomaRentals.Server.Repositories.Interfaces;

namespace OklahomaRentals.Server.Repositories.Implementations
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly ORDbContext _context;

        public PropertyRepository(ORDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<(string Label, int Count)>> GetDependentRecordsAsync(int id)
        {
            var counts = new List<(string, int)>
            {
                ("tenant", await _context.Tenants.CountAsync(t => t.PropertyId == id)),
                ("payment", await _context.Payments.CountAsync(p => p.PropertyId == id)),
                ("maintenance request", await _context.MaintenanceRequests.CountAsync(m => m.PropertyId == id)),
                ("document", await _context.Documents.CountAsync(d => d.PropertyId == id)),
                ("application", await _context.Applications.CountAsync(a => a.PropertyId == id)),
            };
            return counts.Where(c => c.Item2 > 0).ToList();
        }

        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            return await _context.Properties.ToListAsync();
        }

        public async Task<Property?> GetByIdAsync(int id)
        {
            return await _context.Properties.FindAsync(id);
        }

        public async Task<Property> CreateAsync(Property property)
        {
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();
            return property;
        }

        public async Task<Property> UpdateAsync(Property property)
        {
            _context.Entry(property).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return property;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return false;
            }

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Properties.AnyAsync(e => e.Id == id);
        }
    }
}