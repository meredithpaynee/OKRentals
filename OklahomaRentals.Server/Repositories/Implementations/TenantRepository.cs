using Microsoft.EntityFrameworkCore;
using OklahomaRentals.Server.Data;
using OklahomaRentals.Server.Data.Entities;
using OklahomaRentals.Server.Repositories.Interfaces;

namespace OklahomaRentals.Server.Repositories.Implementations
{
    public class TenantRepository : ITenantRepository
    {
        private readonly ORDbContext _context;

        public TenantRepository(ORDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await _context.Tenants.Include(t => t.User).ToListAsync();
        }

        public async Task<Tenant?> GetByIdAsync(int id)
        {
            return await _context.Tenants.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tenant> CreateAsync(Tenant tenant)
        {
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();
            return tenant;
        }

        public async Task<Tenant> UpdateAsync(Tenant tenant)
        {
            _context.Entry(tenant).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return tenant;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return false;
            }

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Tenants.AnyAsync(e => e.Id == id);
        }
    }
}