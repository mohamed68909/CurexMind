using ClincManagement.API.Abstractions;

namespace ClincManagement.API.Services.Interface
{
    public interface IAccountService
    {
        Task<Result<UserProfileResponse>> GetUserProfile(string userId);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<Result> UpdateProfileAsync(string userId, UpdateUserProfileRequest request);
    }
}
