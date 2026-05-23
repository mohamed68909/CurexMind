namespace ClinicManagement.API.Contracts.Appinments.Requests
{
    public class UpdateAppointmentDto : CreateAppointmentDto
    {
        public Guid AppointmentId { get; set; }
    }
}
