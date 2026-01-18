namespace ClincManagement.API.Contracts.Appinments.Respones
{
    public record MyAppointmentResponse(
      Guid AppointmentId,
      string DoctorName,
      string Specialty,
      string ClinicName,
      DateTime Date,
      string Time,
      AppointmentType Type,
      AppointmentStatus Status,
      bool IsPaid,
      bool IsConfirmed
  );

}
