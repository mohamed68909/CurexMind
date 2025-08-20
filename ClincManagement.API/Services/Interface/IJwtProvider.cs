namespace CurexMind.API.Services.Interface
{
    public interface IJwtProvider
    {
        (string token, int expiresIn) GenerateToken(
            ApplicationUser user,
            IEnumerable<string> roles,
            string? doctorId = null,
            IEnumerable<int>? clinicAccess = null,
            IEnumerable<string>? permissions = null
        );
        

        string? ValidateToken(string token);
    }
}
