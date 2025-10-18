using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Authentications.Requests;
using ClincManagement.API.Contracts.Authentications.Respones;
using Google.Apis.Auth.OAuth2.Requests;

namespace ClincManagement.API.Services.Interface
{
    public interface IAuthService
    {

        Task<Result<AuthResponse>> SignInAsync(SignInEmailRequest request, CancellationToken cancellationToken);
        Task<Result<AuthResponse>> SignInGoogleAsync(GoogleSignInRequest request);
        Task<Result<AuthResponse>> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken);
        Task<Result> RevokeAsync(LogOutRequest request);
        Task<Result<AuthResponse>> GenerateRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    }
}
