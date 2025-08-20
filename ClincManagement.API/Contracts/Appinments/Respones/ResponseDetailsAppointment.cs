namespace ClincManagement.API.Contracts.Appinments.Requests
{
    public record ResponseDetailsAppointment
    (
       Guid AppointmentId,
       string patientName,
         string doctorName,
         string clinic,
            DateTime appointmentDate,
            string appointmentTime,
            string appointmentType,
            string status 
        


        );
}
