using System;
using System.Threading.Tasks;
using ClinicManagement.API.Data;
using ClinicManagement.API.Entities;
using ClinicManagement.API.Enums;
using ClinicManagement.API.Services;
using ClinicManagement.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.Tests
{
    public class PatientServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options, null!);
        }

        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        [Fact]
        public async Task GetPatientByIdAsync_ShouldReturnNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var userManagerMock = GetMockUserManager();
            var loggerMock = new Mock<ILogger<PatientService>>();
            var imageFileServiceMock = new Mock<IImageFileService>();

            var service = new PatientService(context, userManagerMock.Object, loggerMock.Object, imageFileServiceMock.Object);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await service.GetPatientByIdAsync(nonExistentId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Patient.NotFound", result.Error.Code);
        }

        [Fact]
        public async Task GetPatientByIdAsync_ShouldReturnSuccess_WhenPatientExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var userManagerMock = GetMockUserManager();
            var loggerMock = new Mock<ILogger<PatientService>>();
            var imageFileServiceMock = new Mock<IImageFileService>();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890"
            };
            var patient = new Patient
            {
                PatientId = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                Gender = Gender.Male,
                DateOfBirth = new DateTime(1990, 1, 1),
                NationalId = "12345678901234",
                Address = "Test Address"
            };

            context.Users.Add(user);
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            var service = new PatientService(context, userManagerMock.Object, loggerMock.Object, imageFileServiceMock.Object);

            // Act
            var result = await service.GetPatientByIdAsync(patient.PatientId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("John Doe", result.Value.FullName);
            Assert.Equal(patient.PatientId, result.Value.PatientId);
        }
    }
}
