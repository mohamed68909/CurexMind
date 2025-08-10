namespace ClincManagement.API.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public Guid PatientId {  get; set; }
        public Guid DoctorId { get; set; }
        public Guid ClinicId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string?  Notes { get; set; }
        public int Duration { get; set; } 


        public AppointmentType Type { get; set; } 
        public AppointmentStatus Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public Patient Patient { get; set; } = default!;
        public Doctor Doctor { get; set; }= default!;
        public Clinic Clinic { get; set; } = default!;




    }
}
