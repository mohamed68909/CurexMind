using ClincManagement.API.Settings;

namespace ClincManagement.API.Entities
{
    public class Appointment : Auditable
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public Guid PatientId {  get; set; }
        public Guid DoctorId { get; set; }
        public Guid ClinicId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; } = string.Empty;
        public string?  Notes { get; set; }
        public int Duration { get; set; } 
       

   
        public AppointmentType Type { get; set; } 
        public AppointmentStatus Status { get; set; }
        public DateTime UpdatedDate { get; set; } 
        public Patient Patient { get; set; } = default!;
        public Doctor Doctor { get; set; }= default!;
        public Clinic Clinic { get; set; } = default!;
        public Payment? Payment { get; set; }







    }
}
