namespace ClincManagement.API.Contracts.Appinments.Requests
{
    public record CreateReAppointment
    (
        Guid PatientId,
        Guid DoctorId,
        Guid ClinicId,
      DateTime AppointmentDate,
     string AppointmentTime,
     int Duration,
     string appointmentType,
        string? status,
        string? notes

    );
}
