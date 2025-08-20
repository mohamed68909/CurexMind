using System.ComponentModel.DataAnnotations;

namespace ClincManagement.API.Settings
{
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";

        [Required(ErrorMessage = "JWT Key is required.")]
        public string Key { get; init; } = string.Empty;

        [Required(ErrorMessage = "JWT Issuer is required.")]
        public string Issuer { get; init; } = string.Empty;

        [Required(ErrorMessage = "JWT Audience is required.")]
        public string Audience { get; init; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Expiration must be greater than 0 minutes.")]
        public int ExpirationPerMinutes { get; init; }
    }
}
