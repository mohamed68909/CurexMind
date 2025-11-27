using ClincManagement.API.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ClincManagement.API.Errors
{
    public static class AppointmentErrors
    {
        public static readonly Error NotFound =
            new("Appointment.NotFound", "Appointment not found in the system", StatusCodes.Status404NotFound);

        public static readonly Error AlreadyExists =
            new("Appointment.AlreadyExists", "An appointment with the same details already exists.", StatusCodes.Status409Conflict);

        public static readonly Error InvalidDate =
            new("Appointment.InvalidDate", "The appointment date is invalid or in the past.", StatusCodes.Status400BadRequest);

        public static readonly Error InvalidDoctor =
            new("Appointment.InvalidDoctor", "The specified doctor is not available or does not exist.", StatusCodes.Status404NotFound);

        public static readonly Error InvalidPatient =
            new("Appointment.InvalidPatient", "The specified patient does not exist in the system.", StatusCodes.Status404NotFound);

        public static readonly Error CreationFailed =
            new("Appointment.CreationFailed", "Failed to create the appointment due to a processing error.", StatusCodes.Status422UnprocessableEntity);

        public static readonly Error UpdateFailed =
            new("Appointment.UpdateFailed", "Failed to update the appointment information.", StatusCodes.Status422UnprocessableEntity);

        public static readonly Error DeletionFailed =
            new("Appointment.DeletionFailed", "Failed to delete the appointment record.", StatusCodes.Status422UnprocessableEntity);
    }
}
