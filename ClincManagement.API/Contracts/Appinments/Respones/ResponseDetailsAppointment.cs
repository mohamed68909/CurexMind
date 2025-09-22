using ClincManagement.API.Contracts.Patient.Responses;

namespace ClincManagement.API.Contracts.Appinments.Respones
{
    public record ResponseDetailsAppointment
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
        IEnumerable<ResponseDetailsAppointment> Data,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    );
}
