using System;
using Microsoft.EntityFrameworkCore;
using OklahomaRentals.Server.Data.Entities;

namespace OklahomaRentals.Server.Data
{

    public class ORDbContext : DbContext
    {
        public ORDbContext(DbContextOptions<ORDbContext> options) : base(options) { }

        public DbSet<Property> Properties { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
