namespace ClincManagement.API.Contracts.Appinments.Requests
{
    public class AppointmentDto
    {
        public Guid AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string ClinicName { get; set; }
        public string AppointmentType { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; } 

        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }
    }
}
