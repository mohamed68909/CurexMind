namespace ClincManagement.API.Contracts.Dashboard
{
    // DashboardSummaryDto.cs
    public class DashboardSummaryDto
    {
        public int TotalPatients { get; set; }
        public int TodayAppointmentsCount { get; set; }
        public int UnpaidInvoicesCount { get; set; }
        public decimal UnpaidInvoicesAmountEGP { get; set; }
        public int NewPatientsToday { get; set; }

        // القوائم التفصيلية
        public List<AppointmentDto> TodayAppointments { get; set; }
        public List<InvoiceFollowUpDto> InvoicesToFollowUp { get; set; }
    }

    // AppointmentDto.cs
    public class AppointmentDto
    {
        public string Time { get; set; }
        public string Patient { get; set; }
        public string Doctor { get; set; }
        public string Status { get; set; }
    }

    // InvoiceFollowUpDto.cs
    public class InvoiceFollowUpDto
    {
        public string Patient { get; set; }
        public decimal AmountEGP { get; set; }
        public string Status { get; set; }
    }
}
