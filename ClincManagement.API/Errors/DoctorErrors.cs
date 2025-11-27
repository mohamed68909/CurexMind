using ClincManagement.API.Abstractions;

namespace ClincManagement.API.Errors
{
    public static class DoctorErrors
    {
        public static readonly Error NotFound = new(
            "Doctor.NotFound",
            "Doctor with the given ID was not found",
            404
        );

        public static readonly Error DuplicateUser = new(
            "Doctor.DuplicateUser",
            "The user is already assigned as a doctor",
            400
        );

        public static readonly Error ClinicNotFound = new(
            "Doctor.ClinicNotFound",
            "The specified clinic does not exist",
            404
        );

        public static readonly Error CreateFailed = new(
            "Doctor.CreateFailed",
            "Failed to create doctor",
            500
        );

        public static readonly Error UpdateFailed = new(
            "Doctor.UpdateFailed",
            "Failed to update doctor",
            500
        );

        public static readonly Error DeleteFailed = new(
            "Doctor.DeleteFailed",
            "Failed to delete doctor",
            500
        );
        public static readonly Error UserNotFound = new(
    "Doctor.UserNotFound",
    "The specified user does not exist",
    404
);

    }
}
