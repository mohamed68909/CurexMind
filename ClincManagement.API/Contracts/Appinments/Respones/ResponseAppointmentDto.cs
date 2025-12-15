namespace ClincManagement.API.Contracts.Appinments.Respones
{
    public class ResponserAppointmentDto
    {
        public Guid AppointmentId { get; set; }
        public string Massage { get; set; } = "Appointment Update Successfully";

    }
}
