
using ClinicManagement.API.Abstractions;
using ClinicManagement.API.Abstractions.Consts;
using ClinicManagement.API.Contracts.Authentications.Requests;
using ClinicManagement.API.Contracts.Authentications.Respones;
using ClinicManagement.API.Errors;
using ClinicManagement.API.Helpers;
using ClinicManagement.API.Services.Interface;
using ClinicManagement.API.Services.Interface;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Requests;
using Mapster;
using System.Security.Cryptography;

namespace ClinicManagement.API.Services;

public class AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtProvider jwtProvider,
        SignInManager<ApplicationUser> signInManager,
        IUserHelpers userHelpers,
        ApplicationDbContext context
    )
    : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private const int _refreshTokenExpirationDays = 30;
    private readonly IUserHelpers _userHelpers = userHelpers;
    private readonly ApplicationDbContext _context = context;


    public async Task<Result<AuthResponse>> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken = default)
    {

        var emailIsExists = await _userManager.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailIsExists)
            return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();
        user.UserName = _userHelpers.GetUserName(request.Email);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description));
        }
        var roleResult = await _userManager.AddToRoleAsync(user, DefaultRoles.Patient.Name);

        if (!roleResult.Succeeded)
        {
            var error = roleResult.Errors.First();
            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description));
        }

        // Create a linked Patient record so the JWT token includes a valid patientId claim.
        // Without this, newly registered users cannot use the patient portal (appointments, invoices, etc.)
        // because all patient routes enforce HasAccessToPatient(patientId) which reads the patientId claim.
        var patient = new Patient
        {
            PatientId = Guid.NewGuid(),
            UserId = user.Id,
            Gender = Gender.Other,
            SocialStatus = SocialStatus.Single,
            DateOfBirth = DateTime.UtcNow.AddYears(-18), // Default placeholder — patient can update their profile later
            NationalId = string.Empty,
            Address = request.Address,
            IsDeleted = false
        };

        await _context.Patients.AddAsync(patient, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(await GetAuthResponse(user));
    }

    public async Task<Result<AuthResponse>> SignInAsync(SignInEmailRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(request.Email.ToLowerInvariant()) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.UserDisabled);
        var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);
        if (result.Succeeded)
            return Result.Success(await GetAuthResponse(user));
        return result.IsLockedOut
           ? Result.Failure<AuthResponse>(UserErrors.UserLockedOut)
           : Result.Failure<AuthResponse>(UserErrors.InvalidCredentials with { StatusCode = StatusCodes.Status400BadRequest });
    }


    public async Task<Result> RevokeAsync(LogOutRequest request)
    {
        var userId = _jwtProvider.ValidateToken(request.Token);
        if (userId is null)
            return Result.Failure(UserErrors.InvalidToken);

        // Fix: Use EF Core directly with Include to eager-load RefreshTokens.
        // _userManager.FindByIdAsync does NOT load related collections.
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        if (user.IsDisabled)
            return Result.Failure(UserErrors.UserDisabled);

        var refreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == request.RefreshToken);
        if (refreshToken is null)
            return Result.Failure(UserErrors.InvalidToken);

        if (refreshToken.RevokedOn is not null)
            return Result.Failure(UserErrors.RefreshTokenAlreadyRevoked);

        if (refreshToken.ExpiresOn <= DateTime.UtcNow)
            return Result.Failure(UserErrors.RefreshTokenExpired);

        refreshToken.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        return Result.Success();
    }
    public async Task<Result<AuthResponse>> GenerateRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == request.RefreshToken), cancellationToken);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.UserDisabled);

        if (await _userManager.IsLockedOutAsync(user))
            return Result.Failure<AuthResponse>(UserErrors.UserLockedOut);

        var refreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == request.RefreshToken);
        if (refreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidToken);

        if (refreshToken.RevokedOn is not null)
            return Result.Failure<AuthResponse>(UserErrors.RefreshTokenAlreadyRevoked);

        if (refreshToken.ExpiresOn <= DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.RefreshTokenExpired);

        refreshToken.RevokedOn = DateTime.UtcNow;
        var response = await GetAuthResponse(user);
        return Result.Success(response);
    }

    private async Task<IEnumerable<string>> GetRolesAsync(ApplicationUser user) =>
     await _userManager.GetRolesAsync(user);
    private static string GenerateRefreshToken() =>
    Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    private async Task<AuthResponse> GetAuthResponse(ApplicationUser user)
    {
        var userRoles = await GetRolesAsync(user);

        string? patientId = null;
        string? doctorId = null;

        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == user.Id);
        if (patient != null)
        {
            patientId = patient.PatientId.ToString();
        }

        var doctor = await _context.Doctors
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.UserId == user.Id);
        if (doctor != null)
        {
            doctorId = doctor.Id.ToString();
        }

        var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, doctorId, patientId);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });
        await _userManager.UpdateAsync(user);
        return new AuthResponse(
            user.Id,
            user.Email!,
            user.FullName,

            token,
            expiresIn,
            refreshToken,
            refreshTokenExpiration
        );
    }


}







