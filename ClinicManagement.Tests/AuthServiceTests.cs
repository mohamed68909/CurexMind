using System;
using System.Threading.Tasks;
using ClinicManagement.API.Data;
using ClinicManagement.API.Entities;
using ClinicManagement.API.Helpers;
using ClinicManagement.API.Services;
using ClinicManagement.API.Services.Interface;
using ClinicManagement.API.Contracts.Authentications.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ClinicManagement.Tests
{
    public class AuthServiceTests
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

        private Mock<SignInManager<ApplicationUser>> GetMockSignInManager(UserManager<ApplicationUser> userManager)
        {
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            return new Mock<SignInManager<ApplicationUser>>(
                userManager,
                contextAccessorMock.Object,
                claimsFactoryMock.Object,
                null!, null!, null!, null!);
        }

        [Fact]
        public async Task SignInAsync_ShouldReturnInvalidCredentials_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var userManagerMock = GetMockUserManager();
            var jwtProviderMock = new Mock<IJwtProvider>();
            var signInManagerMock = GetMockSignInManager(userManagerMock.Object);
            var userHelpersMock = new Mock<IUserHelpers>();

            // Mock FindByEmailAsync to return null
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null!);

            var service = new AuthService(
                userManagerMock.Object,
                jwtProviderMock.Object,
                signInManagerMock.Object,
                userHelpersMock.Object,
                context
            );

            var request = new SignInEmailRequest("nonexistent@example.com", "Password123!");

            // Act
            var result = await service.SignInAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Auth.InvalidCredentials", result.Error.Code);
        }
    }
}
