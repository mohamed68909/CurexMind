using ClincManagement.API.Abstractions;

namespace ClincManagement.API.Errors
{
    public static class PatientErrors
    {
        public static readonly Error NotFound =
            new("Patient.NotFound", "Patient not found in the system", StatusCodes.Status404NotFound);

        public static readonly Error NoAppointments =
            new("Patient.NoAppointments", "This patient has no appointments", StatusCodes.Status404NotFound);

        public static readonly Error DuplicateNationalId =
            new("Patient.DuplicateNationalId", "A patient with the same National ID already exists.", StatusCodes.Status409Conflict);

        public static readonly Error InvalidData =
            new("Patient.InvalidData", "Patient data provided is invalid or incomplete.", StatusCodes.Status400BadRequest);

        public static readonly Error CreationFailed =
            new("Patient.CreationFailed", "Failed to create a new patient due to a processing error.", StatusCodes.Status422UnprocessableEntity);

        public static readonly Error UpdateFailed =
            new("Patient.UpdateFailed", "Failed to update patient information.", StatusCodes.Status422UnprocessableEntity);

        public static readonly Error DeletionFailed =
            new("Patient.DeletionFailed", "Failed to delete the patient record.", StatusCodes.Status422UnprocessableEntity);
    }
}
