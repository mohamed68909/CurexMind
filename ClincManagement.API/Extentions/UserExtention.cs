using System.Security.Claims;

namespace ClincManagement.API.Extentions
{
    public static class UserExtention
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User ID claim is missing.");

            return userId;
        }

        public static bool IsAdmin(this ClaimsPrincipal user) =>
            user.IsInRole("Admin");

        public static bool IsPatient(this ClaimsPrincipal user) =>
            user.IsInRole("Patient");
    }
}