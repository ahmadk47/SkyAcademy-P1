using EmployeeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EmployeeManagementSystem
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<VacationType> VacationTypes { get; set; }
        public DbSet<RequestState> RequestStates { get; set; }
        public DbSet<VacationRequest> VacationRequests { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-QBNQ5B3\\SQLEXPRESS;Database=EmployeeVacationDB;Trusted_Connection=True;TrustServerCertificate=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
            .Property(e => e.Salary)
            .HasColumnType("decimal(18,2)");
            // Configure relationships
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.ReportedTo)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ReportedToEmployeeNumber)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<VacationRequest>()
            //    .HasOne(v => v.ApprovedBy)
            //    .WithMany()
            //    .HasForeignKey(v => v.ApprovedBy);

            //modelBuilder.Entity<VacationRequest>()
            //    .HasOne(v => v.DeclinedBy)
            //    .WithMany()
            //    .HasForeignKey(v => v.DeclinedBy);

            // Seed initial data
            modelBuilder.Entity<RequestState>().HasData(
                new RequestState { StateId = 1, StateName = "Submitted" },
                new RequestState { StateId = 2, StateName = "Approved" },
                new RequestState { StateId = 3, StateName = "Declined" }
            );

            modelBuilder.Entity<VacationType>().HasData(
                new VacationType { VacationTypeCode = "S", VacationTypeName = "Sick" },
                new VacationType { VacationTypeCode = "U", VacationTypeName = "Unpaid" },
                new VacationType { VacationTypeCode = "A", VacationTypeName = "Annual" },
                new VacationType { VacationTypeCode = "O", VacationTypeName = "Day Off" },
                new VacationType { VacationTypeCode = "B", VacationTypeName = "Business Trip" }
            );
        }
    }
}
