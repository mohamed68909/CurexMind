using System;
using System.Threading.Tasks;
using ClinicManagement.API.Data;
using ClinicManagement.API.Entities;
using ClinicManagement.API.Enums;
using ClinicManagement.API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClinicManagement.Tests
{
    public class InvoiceServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options, null!);
        }

        [Fact]
        public async Task GetInvoiceDetailsAsync_ShouldReturnNotFound_WhenInvoiceDoesNotExist()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new InvoiceService(context);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await service.GetInvoiceDetailsAsync(nonExistentId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invoice.NotFound", result.Error.Code);
        }

        [Fact]
        public async Task GetInvoiceDetailsAsync_ShouldReturnSuccess_WhenInvoiceExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();

            var patientUser = new ApplicationUser { Id = Guid.NewGuid().ToString(), FullName = "Patient One" };
            var patient = new Patient
            {
                PatientId = Guid.NewGuid(),
                UserId = patientUser.Id,
                User = patientUser,
                Gender = Gender.Male,
                DateOfBirth = new DateTime(2000, 1, 1),
                NationalId = "11111111111111",
                Address = "Test Address"
            };

            var doctorUser = new ApplicationUser { Id = Guid.NewGuid().ToString(), FullName = "Doctor House" };
            var clinic = new Clinic { Id = Guid.NewGuid(), Name = "St. Jude Clinic", Location = "Road 5", Description = "A medical clinic", IsActive = true };
            var doctor = new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = doctorUser.FullName,
                UserId = doctorUser.Id,
                User = doctorUser,
                ClinicId = clinic.Id,
                Clinic = clinic
            };

            var serviceType = new ServiceType
            {
                Id = Guid.NewGuid(),
                Name = "Consultation"
            };

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceDate = DateTime.UtcNow,
                InvoiceNumber = "INV-12345",
                PatientId = patient.PatientId,
                Patient = patient,
                DoctorId = doctor.Id,
                Doctor = doctor,
                ClinicId = clinic.Id,
                Clinic = clinic,
                ServiceTypeId = serviceType.Id,
                ServiceType = serviceType,
                TotalAmountEGP = 500,
                FinalAmountEGP = 500,
                PaymentMethod = "Cash",
                Status = InvoiceStatus.Paid
            };

            context.Users.AddRange(patientUser, doctorUser);
            context.Clinics.Add(clinic);
            context.Patients.Add(patient);
            context.Doctors.Add(doctor);
            context.Set<ServiceType>().Add(serviceType);
            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();

            var service = new InvoiceService(context);

            // Act
            var result = await service.GetInvoiceDetailsAsync(invoice.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(invoice.Id, result.Value.InvoiceId);
            Assert.Equal("Patient One", result.Value.Patient.Name);
            Assert.Equal("Doctor House", result.Value.Doctor.Name);
        }
    }
}
