namespace CurexMind.API.Contracts.Authentications.Requests
{
    public record LogOutRequest(
     string Token,
     string RefreshToken
 );
}
