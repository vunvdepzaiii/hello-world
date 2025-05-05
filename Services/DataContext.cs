using API.Common;
using API.Entities;
using API.Services.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Services
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
           : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<RoleUser> RoleUsers { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<VehicleFee> VehicleFees { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<WaterBill> WaterBills { get; set; }
        public DbSet<ManagementBill> ManagementBills { get; set; }
        public DbSet<VehicleBill> VehicleBills { get; set; }
        public DbSet<OtherBill> OtherBills { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

                string ConnectionString = configuration.GetSection("SQLServer").Value!;
                optionsBuilder.UseSqlServer(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<RoleUser>().ToTable("RoleUser");
            modelBuilder.Entity<Apartment>().ToTable("Apartment");
            modelBuilder.Entity<VehicleFee>().ToTable("VehicleFee");
            modelBuilder.Entity<Building>().ToTable("Building");
            modelBuilder.Entity<WaterBill>().ToTable("WaterBill");
            modelBuilder.Entity<ManagementBill>().ToTable("ManagementBill");
            modelBuilder.Entity<VehicleBill>().ToTable("VehicleBill");
            modelBuilder.Entity<OtherBill>().ToTable("OtherBill");
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Trước khi lưu, xử lý các entity đang thay đổi
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is GeneralViewModel trackable)
                {
                    var now = DateTime.UtcNow;

                    if (entry.State == EntityState.Added)
                    {
                        trackable.CreatedDate = now;
                        trackable.CreatedBy = ServiceCommon.GetCurrentUserClaim(ClaimTypes.Name);
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        trackable.ModifyDate = now;
                        trackable.ModifyBy = ServiceCommon.GetCurrentUserClaim(ClaimTypes.Name);
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
