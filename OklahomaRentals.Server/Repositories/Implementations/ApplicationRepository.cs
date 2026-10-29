using Microsoft.EntityFrameworkCore;
using OklahomaRentals.Server.Data;
using OklahomaRentals.Server.Data.Entities;
using OklahomaRentals.Server.Repositories.Interfaces;

namespace OklahomaRentals.Server.Repositories.Implementations
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ORDbContext _context;

        public ApplicationRepository(ORDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Application>> GetAllAsync()
        {
            return await _context.Applications.ToListAsync();
        }

        public async Task<Application?> GetByIdAsync(int id)
        {
            return await _context.Applications.FindAsync(id);
        }

        public async Task<Application> CreateAsync(Application application)
        {
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<Application> UpdateAsync(Application application)
        {
            _context.Entry(application).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
            {
                return false;
            }

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Applications.AnyAsync(e => e.Id == id);
        }
    }
}