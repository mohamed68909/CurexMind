namespace ClincManagement.API.Contracts.Appinments.Requests
{
    public record CreateRequestAppointment
    (
        Guid PatientId,
        Guid DoctorId,
        Guid ClinicId,
      DateTime AppointmentDate,
     string AppointmentTime,
     int Duration,
     AppointmentType appointmentType,
        AppointmentStatus? status,
        string? notes

    );
}
