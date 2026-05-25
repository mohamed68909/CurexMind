using System;
using System.Threading.Tasks;
using ClinicManagement.API.Data;
using ClinicManagement.API.Entities;
using ClinicManagement.API.Services;
using ClinicManagement.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.Tests
{
    public class DoctorServiceTests
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
        public async Task GetAsync_ShouldReturnNotFound_WhenDoctorDoesNotExist()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var userManagerMock = GetMockUserManager();
            var loggerMock = new Mock<ILogger<DoctorService>>();
            var imageFileServiceMock = new Mock<IImageFileService>();

            var service = new DoctorService(context, userManagerMock.Object, loggerMock.Object, imageFileServiceMock.Object);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await service.GetAsync(nonExistentId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Doctor.NotFound", result.Error.Code);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnSuccess_WhenDoctorExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var userManagerMock = GetMockUserManager();
            var loggerMock = new Mock<ILogger<DoctorService>>();
            var imageFileServiceMock = new Mock<IImageFileService>();

            var clinic = new Clinic
            {
                Id = Guid.NewGuid(),
                Name = "General Clinic",
                Description = "A primary clinic",
                Location = "123 Main St",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "Dr. Smith",
                Email = "dr.smith@example.com",
                PhoneNumber = "9876543210"
            };

            var doctor = new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = user.FullName,
                UserId = user.Id,
                User = user,
                ClinicId = clinic.Id,
                Clinic = clinic,
                Specialization = "Cardiology",
                YearsOfExperience = 10,
                Languages = "English",
                Price = 200,
                Bio = "Experienced cardiologist."
            };

            context.Clinics.Add(clinic);
            context.Users.Add(user);
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            var service = new DoctorService(context, userManagerMock.Object, loggerMock.Object, imageFileServiceMock.Object);

            // Act
            var result = await service.GetAsync(doctor.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Dr. Smith", result.Value.FullName);
            Assert.Equal("Cardiology", result.Value.Specialization);
        }
    }
}
