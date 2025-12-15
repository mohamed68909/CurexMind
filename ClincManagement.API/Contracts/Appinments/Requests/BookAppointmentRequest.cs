namespace ClincManagement.API.Contracts.Appinments.Requests
{
    public class BookAppointmentRequest
    {
        public string DoctorId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }

        public string ConsultationType { get; set; } = string.Empty; // Video_Consultation or In_Clinic
        public string ReasonForVisit { get; set; } = string.Empty;
        public string PaymentOption { get; set; } = string.Empty; // Pay_Now or Pay_in_Clinic
        public decimal Amount { get; set; }
    }
}
