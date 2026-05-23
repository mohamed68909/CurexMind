using ClinicManagement.API.Abstractions;
using ClinicManagement.API.Contracts.Authentications.Requests;
using ClinicManagement.API.Contracts.Authentications.Respones;
using Google.Apis.Auth.OAuth2.Requests;

namespace ClinicManagement.API.Services.Interface
{
    public interface IAuthService
    {

        Task<Result<AuthResponse>> SignInAsync(SignInEmailRequest request, CancellationToken cancellationToken);
   
        Task<Result<AuthResponse>> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken);
        Task<Result> RevokeAsync(LogOutRequest request);
        Task<Result<AuthResponse>> GenerateRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    }
}
