using ClincManagement.API.Abstractions;
using ClincManagement.API.Errors;
using ClincManagement.API.Services.Interface;
using Mapster;

namespace ClincManagement.API.Services
{

    public class AccountService(UserManager<ApplicationUser> userManager,
        ApplicationDbContext context
        )
        : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _context = context;
        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            if (await _userManager.FindByIdAsync(userId) is not { } user)
                return Result.Failure(UserErrors.NotFound);

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
                return Result.Success();

            var errors = result.Errors.First();

            return Result.Failure(new Error(errors.Code, errors.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result<UserProfileResponse>> GetUserProfile(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure<UserProfileResponse>(UserErrors.NotFound);

            var query = await _context.Users
                .Where(u => u.Id == userId && !u.IsDisabled)
                .AsNoTracking()
                .ProjectToType<UserProfileResponse>()
                .SingleOrDefaultAsync();

            return query is null
                ? Result.Failure<UserProfileResponse>(UserErrors.NotFound)
                : Result.Success(query);
        }

        public async Task<Result> UpdateProfileAsync(string userId, UpdateUserProfileRequest request)
        {
            var updated = await _userManager.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(setter =>
                    setter
                        .SetProperty(u => u.FullName, request.FullName)

                );
            if (updated == 0)
                return Result.Failure(UserErrors.NotFound);
            return Result.Success();
        }


    }

}
