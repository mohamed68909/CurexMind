namespace ClincManagement.API.Contracts.Patient.Responses
{
    public record ResponseAllAppointmentPatient
    {
        public Guid AppointmentId { get; init; }
        public string DoctorName { get; init; } = string.Empty;
        public string Specialization { get; init; } = string.Empty;
        public DateTime Date { get; init; }
        public TimeSpan Time { get; init; }
        public string VisitType { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string PaymentStatus { get; init; } = string.Empty;
        public string? InvoiceUrl { get; init; }
    }

    public record ResponsePatientAppointments
    {
        public Guid PatientId { get; init; }
        public IEnumerable<ResponseAllAppointmentPatient> Appointments { get; init; }
            = Enumerable.Empty<ResponseAllAppointmentPatient>();
    }
}
