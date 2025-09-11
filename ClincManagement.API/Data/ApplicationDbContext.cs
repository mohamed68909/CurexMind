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
       
            
            var clinic1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var clinic2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

            modelBuilder.Entity<Clinic>().HasData(
                new Clinic
                {
                    Id = clinic1Id,
                    Name = "Future Clinic",
                    Description = "General healthcare and diagnostics",
                    Location = "Cairo",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 01, 01)
                },
                new Clinic
                {
                    Id = clinic2Id,
                    Name = "Smile Dental Center",
                    Description = "Specialized in dental care",
                    Location = "Alexandria",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 01, 01)
                }
            );

           
            var doctor1Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var doctor2Id = Guid.Parse("44444444-4444-4444-4444-444444444444");

            modelBuilder.Entity<Doctor>().HasData(
                new Doctor
                {
                    Id = doctor1Id,
                    FullName = "Dr. Ahmed Ali",
                    Specialization = "Cardiology",
                    userId = "user-1",
                    ClinicId = clinic1Id
                },
                new Doctor
                {
                    Id = doctor2Id,
                    FullName = "Dr. Sara Hassan",
                    Specialization = "Dentistry",
                    userId = "user-2",
                    ClinicId = clinic2Id
                }
            );

           
            var patient1Id = Guid.Parse("55555555-5555-5555-5555-555555555555");

            modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    PatientId = patient1Id,
                    NationalId = "29812345678901",
                    Gender = Gender.Male,
                    Notes = "Heart condition",
                    DateOfBirth = new DateTime(1998, 5, 20),
                    ProfileImageUrl = "",
                    CreatedDate = new DateTime(2025, 01, 01),
                    UpdatedDate = new DateTime(2025, 01, 01),
                    IsActive = true,
                    SocialStatus = SocialStatus.Single,
                    UserId = "user-3"
                }
            );

            var review1Id = Guid.Parse("77777777-7777-7777-7777-777777777777");

            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    Id = review1Id,
                    DoctorId = doctor1Id,
                    PatientId = patient1Id,
                    ClinicId = clinic1Id,
                    Rating = 5,
                    Comment = "Excellent doctor!",
                    CreatedAt = new DateTime(2025, 01, 01)
                }
            );
        

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Operation> Operations => Set<Operation>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Stay> Stays => Set<Stay>();
        public DbSet<Clinic> Clinics => Set<Clinic>();
        public DbSet<VitalSigns> VitalSigns => Set<VitalSigns>();
        public DbSet<Review> Reviews => Set<Review>();



    }
}
