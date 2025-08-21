using ClincManagement.API.Contracts.Authentications.Requests;
using ClincManagement.API.Contracts.Authentications.Respones;
using ClincManagement.API.Errors;
using ClincManagement.API.Services.Interface;
using CurexMind.API.Services.Interface;
using Google.Apis.Auth;
using System.Security.Cryptography;
using System.Text;

namespace HospitalManagement.API.Services;

//public class AuthService(
//        UserManager<ApplicationUser> userManager,
//        IJwtProvider jwtProvider,
//        SignInManager<ApplicationUser> signInManager
//    )
//    : IAuthService
//{
//    private readonly UserManager<ApplicationUser> _userManager = userManager;
//    private readonly IJwtProvider _jwtProvider = jwtProvider;
//    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
//    private const int _refreshTokenExpirationDays = 30;

 
//    public async Task<Result<AuthResponse>> SignUpPatientAsync(SignUpRequest request, CancellationToken cancellationToken = default!)
//    {
//        var emailIsExists = await _userManager.Users
//            .AnyAsync(u => u.Email == request.Email, cancellationToken);

//        if (emailIsExists)
//            return Result.Failure<AuthResponse>(UserErrors.DublicatedEmail);

//        var patient = request.Adapt<ApplicationUser>();
//        patient.UserName = GetUserName(request.Email);

//        var result = await _userManager.CreateAsync(patient, request.Password);
//        if (!result.Succeeded)
//        {
//            var error = result.Errors.First();
//            return Result.Failure<AuthResponse>(
//                new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
//        }

      
//        await _userManager.AddToRoleAsync(patient, DefaultRoles.Patient.Name);

//        return Result.Success(await GetAuthResponse(patient));
//    }

  
//    public async Task<Result<AuthResponse>> SignInAsync(SignInEmailRequest request, CancellationToken cancellationToken = default)
//    {
//        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
//            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

//        if (user.IsDisabled)
//            return Result.Failure<AuthResponse>(UserErrors.UserDisabled);

//        var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);

//        if (result.Succeeded)
//            return Result.Success(await GetAuthResponse(user));

//        return result.IsLockedOut
//           ? Result.Failure<AuthResponse>(UserErrors.UserLockedOut)
//           : Result.Failure<AuthResponse>(UserErrors.InvalidCredentials with
//           { StatusCode = StatusCodes.Status400BadRequest });
//    }

   
//    public async Task<Result<AuthResponse>> GoogleSignInAsync(GoogleSignInRequest request)
//    {
//        if (await GoogleJsonWebSignature.ValidateAsync(request.TokenID) is not { } payload)
//            return Result.Failure<AuthResponse>(UserErrors.InvalidGoogleToken);

//        var user = await _userManager.FindByEmailAsync(payload.Email);

//        if (user is null)
//        {
//            user = new ApplicationUser
//            {
//                UserName = GetUserName(payload.Email),
//                Email = payload.Email,
//                FirstName = payload.GivenName ?? string.Empty,
//                LastName = payload.FamilyName ?? string.Empty,
//                EmailConfirmed = true
//            };

//            var result = await _userManager.CreateAsync(user);
//            if (!result.Succeeded)
//                return Result.Failure<AuthResponse>(UserErrors.UserCreationFailed);

        
//            await _userManager.AddToRoleAsync(user, DefaultRoles.Patient.Name);

//            var loginInfo = new UserLoginInfo(Providers.Google, payload.Subject, Providers.Google);
//            var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
//            if (!addLoginResult.Succeeded)
//                return Result.Failure<AuthResponse>(UserErrors.ExternalLoginFailed);
//        }
//        else
//        {
//            if (user.IsDisabled)
//                return Result.Failure<AuthResponse>(UserErrors.UserDisabled);

//            if (await _userManager.IsLockedOutAsync(user))
//                return Result.Failure<AuthResponse>(UserErrors.UserLockedOut);

//            var logins = await _userManager.GetLoginsAsync(user);
//            if (!logins.Any(x => x.LoginProvider == Providers.Google))
//            {
//                var loginInfo = new UserLoginInfo(Providers.Google, payload.Subject, Providers.Google);
//                var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
//                if (!addLoginResult.Succeeded)
//                    return Result.Failure<AuthResponse>(UserErrors.ExternalLoginFailed);
//            }
//        }

//        return Result.Success(await GetAuthResponse(user));
//    }

//    // ✅ Utilities
//    private async Task<IEnumerable<string>> GetRolesAsync(ApplicationUser user) =>
//         await _userManager.GetRolesAsync(user);

//    private static string GetFullName(string firstName, string lastName)
//    {
//        var builder = new StringBuilder();
//        if (!string.IsNullOrWhiteSpace(firstName))
//            builder.Append(firstName.Trim());

//        if (!string.IsNullOrWhiteSpace(lastName))
//        {
//            if (builder.Length > 0)
//                builder.Append(' ');

//            builder.Append(lastName.Trim());
//        }
//        return builder.ToString();
//    }

//    private static string GenerateRefreshToken() =>
//        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

//    private async Task<AuthResponse> GetAuthResponse(ApplicationUser user)
//    {
//        var userRoles = await GetRolesAsync(user);
//        var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles);

//        var refreshToken = GenerateRefreshToken();
//        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

//        user.RefreshTokens.Add(new RefreshToken
//        {
//            Token = refreshToken,
//            ExpiresOn = refreshTokenExpiration
//        });

//        await _userManager.UpdateAsync(user);

//        return new AuthResponse(
//            user.Id,
//            user.Email!,
//            GetFullName(user.FirstName, user.LastName),
//            token,
//            expiresIn,
//            refreshToken,
//            refreshTokenExpiration
//        );
//    }

//    private static string GetUserName(string? email)
//    {
//        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
//            return string.Empty;

//        return email.Split('@')[0];
//    }

//    public  async Task<Result<AuthResponse>> SignInUserNameAsync(SignInUserNameRequest request, CancellationToken cancellationToken = default!)
//    {
//        if (await _userManager.FindByEmailAsync(request.UserName) is not { } user)
//            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

//        if (user.IsDisabled)
//            return Result.Failure<AuthResponse>(UserErrors.UserDisabled);

//        var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);

//        if (result.Succeeded)
//            return Result.Success(await GetAuthResponse(user));

//        return result.IsLockedOut
//           ? Result.Failure<AuthResponse>(UserErrors.UserLockedOut)
//           : Result.Failure<AuthResponse>(UserErrors.InvalidCredentials with
//           { StatusCode = StatusCodes.Status400BadRequest });
//    }

//    public async Task<Result<AuthResponse>> SignInEmailAsync(SignInEmailRequest request, CancellationToken cancellationToken = default!)
//    {
//        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
//            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

//        if (user.IsDisabled)
//            return Result.Failure<AuthResponse>(UserErrors.UserDisabled);

//        var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);

//        if (result.Succeeded)
//            return Result.Success(await GetAuthResponse(user));

//        return result.IsLockedOut
//           ? Result.Failure<AuthResponse>(UserErrors.UserLockedOut)
//           : Result.Failure<AuthResponse>(UserErrors.InvalidCredentials with
//           { StatusCode = StatusCodes.Status400BadRequest });
//    }



   




//}
