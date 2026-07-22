using CurexMind.API.Abstractions.Consts;
using CurexMind.API.Data;
using CurexMind.API.Entities;
using CurexMind.API.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurexMind.API.Helpers
{
    public class DbSeeder
    {
        public static async Task SeedMockDataAsync(ApplicationDbContext context)
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            // Ensure roles exist in Db
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == DefaultRoles.Admin.Name);
            if (adminRole == null)
            {
                adminRole = new ApplicationRole
                {
                    Id = DefaultRoles.Admin.Id,
                    Name = DefaultRoles.Admin.Name,
                    NormalizedName = DefaultRoles.Admin.Name.ToUpperInvariant(),
                    ConcurrencyStamp = DefaultRoles.Admin.ConcurrencyStamp
                };
                await context.Roles.AddAsync(adminRole);
            }

            var patientRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == DefaultRoles.Patient.Name);
            if (patientRole == null)
            {
                patientRole = new ApplicationRole
                {
                    Id = DefaultRoles.Patient.Id,
                    Name = DefaultRoles.Patient.Name,
                    NormalizedName = DefaultRoles.Patient.Name.ToUpperInvariant(),
                    ConcurrencyStamp = DefaultRoles.Patient.ConcurrencyStamp
                };
                await context.Roles.AddAsync(patientRole);
            }

            await context.SaveChangesAsync();

            // Ensure clinics exist
            var clinics = await context.Clinics.ToListAsync();
            if (!clinics.Any())
            {
                clinics = new List<Clinic>
                {
                    new Clinic { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Main Clinic", Location = "Downtown Medical Center, Floor 3" },
                    new Clinic { Id = Guid.Parse("33333333-3333-3333-3333-333333333002"), Name = "Downtown Clinic", Location = "Tahrir Square Plaza, Tower B" },
                    new Clinic { Id = Guid.Parse("33333333-3333-3333-3333-333333333003"), Name = "Uptown Clinic", Location = "Zamalek 26th July Street" },
                    new Clinic { Id = Guid.Parse("33333333-3333-3333-3333-333333333004"), Name = "Eastside Clinic", Location = "New Cairo District 5" },
                    new Clinic { Id = Guid.Parse("33333333-3333-3333-3333-333333333005"), Name = "West End Clinic", Location = "Sheikh Zayed Arkan Plaza" },
                };
                await context.Clinics.AddRangeAsync(clinics);
                await context.SaveChangesAsync();
            }

            // Ensure ServiceTypes exist
            var serviceTypes = await context.ServiceTypes.ToListAsync();
            if (!serviceTypes.Any())
            {
                serviceTypes = new List<ServiceType>
                {
                    new ServiceType { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Consultation" },
                    new ServiceType { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Installation" },
                    new ServiceType { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Maintenance" },
                    new ServiceType { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Emergency Repair" },
                };
                await context.ServiceTypes.AddRangeAsync(serviceTypes);
                await context.SaveChangesAsync();
            }

            // 1. Seed 50 Doctors
            var currentDoctorsCount = await context.Doctors.CountAsync();
            if (currentDoctorsCount < 50)
            {
                var specializations = new[] { "Cardiology", "Pediatrics", "Dermatology", "Orthopedics", "Neurology", "General Surgery", "Internal Medicine", "Ophthalmology", "ENT", "Psychiatry" };
                var firstNames = new[] { "Ahmed", "Mohamed", "Tarek", "Youssef", "Omar", "Sara", "Nour", "Mona", "Hoda", "Layla", "Khaled", "Mahmoud", "Hassan", "Eman", "Reem" };
                var lastNames = new[] { "Hassan", "Ali", "Ibrahim", "Sayed", "Farouk", "El-Din", "Mostafa", "Salama", "Mansour", "Abdelrahman" };

                int targetDoctors = 50 - currentDoctorsCount;
                var rand = new Random();

                for (int i = 0; i < targetDoctors; i++)
                {
                    var fn = firstNames[rand.Next(firstNames.Length)];
                    var ln = lastNames[rand.Next(lastNames.Length)];
                    var name = $"Dr. {fn} {ln}";
                    var email = $"doctor_{Guid.NewGuid().ToString().Substring(0, 8)}@curexmind.com";
                    var userId = Guid.NewGuid().ToString();

                    var user = new ApplicationUser
                    {
                        Id = userId,
                        UserName = email,
                        NormalizedUserName = email.ToUpperInvariant(),
                        Email = email,
                        NormalizedEmail = email.ToUpperInvariant(),
                        FullName = name,
                        PhoneNumber = $"010{rand.Next(10000000, 99999999)}",
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    user.PasswordHash = hasher.HashPassword(user, "Doctor@123456");

                    await context.Users.AddAsync(user);
                    await context.UserRoles.AddAsync(new IdentityUserRole<string> { UserId = userId, RoleId = adminRole.Id });

                    var clinic = clinics[rand.Next(clinics.Count)];
                    var spec = specializations[rand.Next(specializations.Length)];
                    var doctor = new Doctor
                    {
                        Id = Guid.NewGuid(),
                        FullName = name,
                        Specialization = spec,
                        YearsOfExperience = rand.Next(3, 30),
                        Languages = "Arabic, English",
                        Price = rand.Next(150, 600),
                        Bio = $"Experienced specialist in {spec} with over {rand.Next(5, 25)} years of clinical practice.",
                        ClinicId = clinic.Id,
                        UserId = user.Id
                    };
                    await context.Doctors.AddAsync(doctor);
                }
                await context.SaveChangesAsync();
            }

            var allDoctorInfos = await context.Doctors.Select(d => new { d.Id, d.ClinicId }).ToListAsync();

            // 2. Seed 300 Patients
            var currentPatientsCount = await context.Patients.CountAsync();
            if (currentPatientsCount < 300)
            {
                var patientFirstNames = new[] { "Karim", "Ziad", "Yassen", "Nader", "Hany", "Dalia", "Fatma", "Salma", "Aya", "Noha", "Habiba", "Amr", "Sherif", "Rania", "Bassem" };
                var patientLastNames = new[] { "Fawzy", "Kamel", "Samir", "Ghanem", "Nabil", "Tawfik", "Soliman", "Khafagy", "Osman", "Riad" };
                int targetPatients = 300 - currentPatientsCount;
                var rand = new Random();

                for (int i = 0; i < targetPatients; i++)
                {
                    var fn = patientFirstNames[rand.Next(patientFirstNames.Length)];
                    var ln = patientLastNames[rand.Next(patientLastNames.Length)];
                    var name = $"{fn} {ln}";
                    var email = $"patient_{Guid.NewGuid().ToString().Substring(0, 8)}@curexmind.com";
                    var userId = Guid.NewGuid().ToString();

                    var user = new ApplicationUser
                    {
                        Id = userId,
                        UserName = email,
                        NormalizedUserName = email.ToUpperInvariant(),
                        Email = email,
                        NormalizedEmail = email.ToUpperInvariant(),
                        FullName = name,
                        PhoneNumber = $"011{rand.Next(10000000, 99999999)}",
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    user.PasswordHash = hasher.HashPassword(user, "Patient@123456");

                    await context.Users.AddAsync(user);
                    await context.UserRoles.AddAsync(new IdentityUserRole<string> { UserId = userId, RoleId = patientRole.Id });

                    var patient = new Patient
                    {
                        PatientId = Guid.NewGuid(),
                        UserId = user.Id,
                        Gender = (Gender)rand.Next(1, 3),
                        SocialStatus = (SocialStatus)rand.Next(1, 3),
                        DateOfBirth = DateTime.UtcNow.AddYears(-rand.Next(18, 75)).AddDays(-rand.Next(0, 365)),
                        NationalId = $"2{rand.Next(80, 99):D2}{rand.Next(1, 13):D2}{rand.Next(1, 31):D2}01{rand.Next(1000, 9999)}",
                        Address = $"{rand.Next(1, 100)} Street {rand.Next(1, 20)}, Cairo, Egypt",
                        Notes = "Patient registered in system batch seeding.",
                        IsDeleted = false
                    };
                    await context.Patients.AddAsync(patient);
                }
                await context.SaveChangesAsync();
            }

            var allPatientInfos = await context.Patients.Select(p => new { p.PatientId, p.UserId }).ToListAsync();

            // 3. Seed 1000 Appointments in chunks
            var currentApptsCount = await context.Appointments.CountAsync();
            if (currentApptsCount < 1000)
            {
                int targetAppts = 1000 - currentApptsCount;
                var rand = new Random();
                var batch = new List<Appointment>();

                for (int i = 0; i < targetAppts; i++)
                {
                    var patient = allPatientInfos[rand.Next(allPatientInfos.Count)];
                    var doctor = allDoctorInfos[rand.Next(allDoctorInfos.Count)];
                    var daysOffset = rand.Next(-90, 60);
                    var apptDate = DateTime.UtcNow.Date.AddDays(daysOffset);
                    var hour = rand.Next(9, 18);
                    var timeSpan = new TimeSpan(hour, rand.Next(0, 2) * 30, 0);

                    var appt = new Appointment
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patient.PatientId,
                        DoctorId = doctor.Id,
                        ClinicId = doctor.ClinicId,
                        AppointmentDate = apptDate,
                        AppointmentTime = timeSpan,
                        Duration = 30,
                        Notes = "Regular medical consultation",
                        Type = (AppointmentType)rand.Next(1, 4),
                        Status = (AppointmentStatus)rand.Next(0, 4),
                        PaymentStatus = (PaymentStatus)rand.Next(0, 3)
                    };
                    batch.Add(appt);

                    if (batch.Count >= 250)
                    {
                        await context.Appointments.AddRangeAsync(batch);
                        await context.SaveChangesAsync();
                        batch.Clear();
                    }
                }

                if (batch.Any())
                {
                    await context.Appointments.AddRangeAsync(batch);
                    await context.SaveChangesAsync();
                }
            }

            // 4. Seed 500 Reviews
            var currentReviewsCount = await context.Reviews.CountAsync();
            if (currentReviewsCount < 500)
            {
                int targetReviews = 500 - currentReviewsCount;
                var rand = new Random();
                var comments = new[]
                {
                    "Excellent doctor, very attentive and thorough explanation.",
                    "Great experience at the clinic, minimal waiting time.",
                    "Professional behavior, accurate diagnosis and helpful treatment plan.",
                    "Very friendly staff and clean environment.",
                    "Highly recommended doctor for all family medical care!"
                };
                var batch = new List<Review>();

                for (int i = 0; i < targetReviews; i++)
                {
                    var patient = allPatientInfos[rand.Next(allPatientInfos.Count)];
                    var doctor = allDoctorInfos[rand.Next(allDoctorInfos.Count)];

                    var review = new Review
                    {
                        Id = Guid.NewGuid(),
                        DoctorId = doctor.Id,
                        ClinicId = doctor.ClinicId,
                        UserId = patient.UserId!,
                        Rating = rand.Next(4, 6),
                        Comment = comments[rand.Next(comments.Length)],
                        CreatedAt = DateTime.UtcNow.AddDays(-rand.Next(1, 120))
                    };
                    batch.Add(review);

                    if (batch.Count >= 250)
                    {
                        await context.Reviews.AddRangeAsync(batch);
                        await context.SaveChangesAsync();
                        batch.Clear();
                    }
                }

                if (batch.Any())
                {
                    await context.Reviews.AddRangeAsync(batch);
                    await context.SaveChangesAsync();
                }
            }

            // 5. Seed 300 Invoices
            var currentInvoicesCount = await context.Invoices.CountAsync();
            if (currentInvoicesCount < 300)
            {
                int targetInvoices = 300 - currentInvoicesCount;
                var rand = new Random();
                var batch = new List<Invoice>();

                for (int i = 0; i < targetInvoices; i++)
                {
                    var patient = allPatientInfos[rand.Next(allPatientInfos.Count)];
                    var doctor = allDoctorInfos[rand.Next(allDoctorInfos.Count)];
                    var sType = serviceTypes[rand.Next(serviceTypes.Count)];
                    var total = (decimal)rand.Next(200, 1500);
                    var discount = (decimal)rand.Next(0, 50);
                    var finalAmt = total - discount;
                    var isPaid = rand.Next(0, 2) == 1;

                    var invoice = new Invoice
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patient.PatientId,
                        DoctorId = doctor.Id,
                        ClinicId = doctor.ClinicId,
                        ServiceTypeId = sType.Id,
                        InvoiceNumber = $"INV-{DateTime.UtcNow.Year}-{rand.Next(10000, 99999)}",
                        InvoiceDate = DateTime.UtcNow.AddDays(-rand.Next(1, 90)),
                        VisitDate = DateTime.UtcNow.AddDays(-rand.Next(1, 90)),
                        VisitTime = new TimeOnly(rand.Next(9, 17), 0),
                        TotalAmountEGP = total,
                        DiscountEGP = discount,
                        FinalAmountEGP = finalAmt,
                        PaidAmountEGP = isPaid ? finalAmt : 0,
                        Status = isPaid ? InvoiceStatus.Paid : InvoiceStatus.Due,
                        PaymentMethod = isPaid ? "Cash" : "Credit Card",
                        Notes = "Generated billed service invoice"
                    };
                    batch.Add(invoice);

                    if (batch.Count >= 150)
                    {
                        await context.Invoices.AddRangeAsync(batch);
                        await context.SaveChangesAsync();
                        batch.Clear();
                    }
                }

                if (batch.Any())
                {
                    await context.Invoices.AddRangeAsync(batch);
                    await context.SaveChangesAsync();
                }
            }

            // 6. Seed 50 Stays
            var currentStaysCount = await context.Stays.CountAsync();
            if (currentStaysCount < 50)
            {
                int targetStays = 50 - currentStaysCount;
                var rand = new Random();
                var departments = new[] { "Cardiology", "Orthopedics", "General Surgery", "ICU", "Pediatrics" };

                for (int i = 0; i < targetStays; i++)
                {
                    var patient = allPatientInfos[rand.Next(allPatientInfos.Count)];
                    var dept = departments[rand.Next(departments.Length)];
                    var roomNum = $"Room {rand.Next(101, 405)}";
                    var bedNum = $"Bed {((char)('A' + rand.Next(0, 3)))}";
                    var isCompleted = rand.Next(0, 2) == 1;
                    var startDate = DateTime.UtcNow.AddDays(-rand.Next(5, 30));

                    var stay = new Stay
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patient.PatientId,
                        Department = dept,
                        RoomNumber = roomNum,
                        BedNumber = bedNum,
                        StayType = (StayType)rand.Next(0, 4),
                        Status = isCompleted ? StayStatus.Completed : StayStatus.Active,
                        StartDate = startDate,
                        EndDate = isCompleted ? startDate.AddDays(rand.Next(1, 7)) : null,
                        Notes = "Inpatient ward admission record"
                    };
                    await context.Stays.AddAsync(stay);
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
