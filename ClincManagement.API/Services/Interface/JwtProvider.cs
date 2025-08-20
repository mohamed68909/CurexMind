using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

using ClincManagement.API.Settings;
using ClincManagement.API.Services.Interface;

namespace CurexMind.API.Services
{
    public class JwtProvider(IOptions<JwtOptions> jwtOptions) : IJwtProvider
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        public (string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, string? doctorId = null, IEnumerable<int>? clinicAccess = null, IEnumerable<string>? permissions = null)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("fullName", user.FullName)
            };

            // Roles
            claims.Add(new Claim("roles", JsonSerializer.Serialize(roles), JsonClaimValueTypes.JsonArray));

            // DoctorId Claim
            if (!string.IsNullOrEmpty(doctorId))
                claims.Add(new Claim("doctorId", doctorId));

            // ClinicAccess Claim
            if (clinicAccess != null && clinicAccess.Any())
                claims.Add(new Claim("clinicAccess", JsonSerializer.Serialize(clinicAccess), JsonClaimValueTypes.JsonArray));

            // Permissions Claim
            if (permissions != null && permissions.Any())
                claims.Add(new Claim("permissions", JsonSerializer.Serialize(permissions), JsonClaimValueTypes.JsonArray));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationPerMinutes),
                signingCredentials: creds
            );

            return (
                token: new JwtSecurityTokenHandler().WriteToken(token),
                expiresIn: (int)_jwtOptions.ExpirationPerMinutes * 60
            );
        }

        public string? ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var handler = new JwtSecurityTokenHandler();

            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtOptions.Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return ((JwtSecurityToken)validatedToken)
                    .Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
