namespace ClincManagement.API.Contracts.Authentications.Respones
{
    public record AuthResponse(
    string Id,
    string Email,
    string FullName,
    string Token,
    int ExpiresIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);
}
