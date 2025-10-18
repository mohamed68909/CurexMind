namespace ClincManagement.API.Contracts.Appinments.Respones
{
    public record ResponseDetailsAllAppointment
    (
        Guid AppointmentId,
        string PatientName,
        string DoctorName,
        string Clinic,
        DateTime AppointmentDate,
        string AppointmentTime,
        AppointmentType AppointmentType,
        AppointmentStatus Status
    );

    public record PagedAppointmentResponse
    (
        IEnumerable<ResponseDetailsAllAppointment> Data,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    );
}
