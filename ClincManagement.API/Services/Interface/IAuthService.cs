using ClincManagement.API.Contracts.Authentications.Requests;
using ClincManagement.API.Contracts.Authentications.Respones;

namespace ClincManagement.API.Services.Interface
{
   public interface IAuthService
    {
        Task<AuthResponse> SignInUserNameAsync(SignInUserNameRequest request, CancellationToken cancellationToken);
        Task<AuthResponse> SignInEmailAsync(SignInEmailRequest request, CancellationToken cancellationToken);
        Task<AuthResponse> GoogleSignInAsync(GoogleSignInRequest request);
        Task<AuthResponse> SignUpPatientAsync(SignUpRequest request,CancellationToken cancellationToken);

    }
}
