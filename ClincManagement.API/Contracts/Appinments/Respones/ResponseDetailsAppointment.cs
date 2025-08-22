using ClincManagement.API.Contracts.Patient.Responses;

namespace ClincManagement.API.Contracts.Appinments.Respones
{
    public record ResponseDetailsAppointment
    (
       Guid AppointmentId,
       string patientName,
         string doctorName,
         string clinic,
            DateTime appointmentDate,
            string appointmentTime,
            AppointmentType appointmentType,
            AppointmentStatus status 
        


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
