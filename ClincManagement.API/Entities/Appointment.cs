using ClincManagement.API.Settings;

namespace ClincManagement.API.Entities
{
    public class Appointment : Auditable
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid ClinicId { get; set; }


        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; } 
        public int Duration { get; set; }

        public string? Notes { get; set; }

        public AppointmentType Type { get; set; }
        public AppointmentStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        // Navigation
        public Patient Patient { get; set; } = default!;
        public Doctor Doctor { get; set; } = default!;
        public Clinic Clinic { get; set; } = default!;
        public ApplicationUser user { get; set; } = default!;
        public Invoice? Invoice { get; set; }
        // Optional
        public Payment? Payment { get; set; }
    }
}
