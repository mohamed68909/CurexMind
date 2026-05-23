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
    public class DashboardServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options, null!);
        }

        [Fact]
        public async Task GetReceptionistSummaryAsync_ShouldReturnSuccess_WhenDatabaseIsEmpty()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new DashboardService(context);
            var userId = Guid.NewGuid();

            // Act
            var result = await service.GetReceptionistSummaryAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(0, result.Value.TotalPatients);
            Assert.Equal(0, result.Value.TodayAppointmentsCount);
            Assert.Equal(0, result.Value.UnpaidInvoicesCount);
            Assert.Equal(0, result.Value.UnpaidInvoicesAmountEGP);
            Assert.Equal(0, result.Value.NewPatientsToday);
            Assert.Empty(result.Value.TodayAppointments);
            Assert.Empty(result.Value.InvoicesToFollowUp);
        }

        [Fact]
        public async Task GetReceptionistSummaryAsync_ShouldReturnCorrectCounts_WhenDataExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new DashboardService(context);
            var userId = Guid.NewGuid();

            var patient = new Patient
            {
                PatientId = Guid.NewGuid(),
                NationalId = "12345678901234",
                Gender = Gender.Male,
                Address = "Test Address",
                DateOfBirth = new DateTime(1990, 1, 1),
                CreatedOn = DateTime.UtcNow
            };

            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetReceptionistSummaryAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value.TotalPatients);
            Assert.Equal(1, result.Value.NewPatientsToday);
        }
    }
}
