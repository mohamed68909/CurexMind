namespace ClincManagement.API.Contracts.Appointments.Responses
{
    namespace ClincManagement.API.Contracts.Appointments.Responses
    {
        public class AppointmentDetailsResponse
        {
            public string AppointmentId { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;

            public string ReferenceCode { get; set; } = string.Empty;
            public string InvoiceId { get; set; } = string.Empty;
            public string PaymentStatus { get; set; } = string.Empty;

            public DoctorDetails DoctorDetails { get; set; } = new();
            public PatientSummary PatientSummary { get; set; } = new();

            public BookingTime BookingTime { get; set; } = new();
           

            public string Notes { get; set; } = string.Empty;

            public List<ActivityLogItem> ActivityLog { get; set; } = new();
        }

        public class PatientSummary
        {
            public string PatientId { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Gender { get; set; } = string.Empty;
            public int Age { get; set; }

            public string Phone { get; set; } = string.Empty;
            public string NationalId { get; set; } = string.Empty;

            public UploadedFile? ProfileImageUrl { get; set; }
        }

        public class DoctorDetails
    {
        public string Name { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;
    }
        public class BookingTime
        {
            public string Date { get; set; } = string.Empty;   // yyyy-MM-dd
            public string Time { get; set; } = string.Empty;   // hh:mm tt
            public int DurationMinutes { get; set; }
        }
        public class ActivityLogItem
        {
            public string Action { get; set; } = string.Empty;
            public string By { get; set; } = string.Empty;
            public string Date { get; set; } = string.Empty; 
        }
    }


}
