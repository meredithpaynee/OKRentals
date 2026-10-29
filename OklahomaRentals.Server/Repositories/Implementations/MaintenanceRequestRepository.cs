using Microsoft.EntityFrameworkCore;
using OklahomaRentals.Server.Data;
using OklahomaRentals.Server.Data.Entities;
using OklahomaRentals.Server.Repositories.Interfaces;

namespace OklahomaRentals.Server.Repositories.Implementations
{
    public class MaintenanceRequestRepository : IMaintenanceRequestRepository
    {
        private readonly ORDbContext _context;

        public MaintenanceRequestRepository(ORDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MaintenanceRequest>> GetAllAsync()
        {
            return await _context.MaintenanceRequests.ToListAsync();
        }

        public async Task<MaintenanceRequest?> GetByIdAsync(int id)
        {
            return await _context.MaintenanceRequests.FindAsync(id);
        }

        public async Task<MaintenanceRequest> CreateAsync(MaintenanceRequest maintenanceRequest)
        {
            _context.MaintenanceRequests.Add(maintenanceRequest);
            await _context.SaveChangesAsync();
            return maintenanceRequest;
        }

        public async Task<MaintenanceRequest> UpdateAsync(MaintenanceRequest maintenanceRequest)
        {
            _context.Entry(maintenanceRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return maintenanceRequest;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var maintenanceRequest = await _context.MaintenanceRequests.FindAsync(id);
            if (maintenanceRequest == null)
            {
                return false;
            }

            _context.MaintenanceRequests.Remove(maintenanceRequest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MaintenanceRequests.AnyAsync(e => e.Id == id);
        }
    }
}