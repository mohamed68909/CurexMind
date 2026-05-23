using System.Security.Claims;

namespace ClinicManagement.API.Extentions
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

        public static Guid? GetPatientId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirstValue("patientId");
            return Guid.TryParse(claim, out var guid) ? guid : null;
        }

        public static bool HasAccessToPatient(this ClaimsPrincipal user, Guid patientId)
        {
            if (user.IsAdmin()) return true;
            return user.GetPatientId() == patientId;
        }
    }
}