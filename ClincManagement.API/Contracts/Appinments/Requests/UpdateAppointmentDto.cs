namespace ClincManagement.API.Contracts.Appinments.Requests
{
    public class UpdateAppointmentDto : CreateAppointmentDto
    {
        public Guid AppointmentId { get; set; }
    }
}
