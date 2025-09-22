using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace ClincManagement.API.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser,ApplicationRole,string>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);




            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Operation> Operations => Set<Operation>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Stay> Stays => Set<Stay>();
        public DbSet<Clinic> Clinics => Set<Clinic>();
 
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Payment> Payments => Set<Payment>();



    }
}
