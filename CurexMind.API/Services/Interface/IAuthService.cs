using CurexMind.API.Abstractions;
using CurexMind.API.Contracts.Authentications.Requests;
using CurexMind.API.Contracts.Authentications.Respones;
using Google.Apis.Auth.OAuth2.Requests;

namespace CurexMind.API.Services.Interface
{
    public interface IAuthService
    {

        Task<Result<AuthResponse>> SignInAsync(SignInEmailRequest request, CancellationToken cancellationToken);
   
        Task<Result<AuthResponse>> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken);
        Task<Result> RevokeAsync(LogOutRequest request);
        Task<Result<AuthResponse>> GenerateRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    }
}
