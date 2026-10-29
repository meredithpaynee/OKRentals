using Microsoft.EntityFrameworkCore;
using OklahomaRentals.Server.Data;
using OklahomaRentals.Server.Data.Entities;
using OklahomaRentals.Server.Repositories.Interfaces;

namespace OklahomaRentals.Server.Repositories.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ORDbContext _context;

        public AdminRepository(ORDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _context.Admins.ToListAsync();
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            return await _context.Admins.FindAsync(id);
        }

        public async Task<Admin> CreateAsync(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task<Admin> UpdateAsync(Admin admin)
        {
            _context.Entry(admin).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return false;
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Admins.AnyAsync(e => e.Id == id);
        }
    }
}