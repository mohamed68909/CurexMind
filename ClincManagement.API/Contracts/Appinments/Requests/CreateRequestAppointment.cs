namespace ClincManagement.API.Contracts.Appinments.Requests
{

    public class CreateAppointmentDto
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid ClinicId { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus? Status { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }
    }
}
