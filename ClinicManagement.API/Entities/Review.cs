namespace ClinicManagement.API.Entities
{
    public class Review
    {

        public Guid Id { get; set; }



        public int Rating { get; set; } 


        public string Comment { get; set; } = string.Empty;


        public DateTime CreatedAt { get; set; }

        public Guid? ClinicId { get; set; }
        public Clinic Clinic { get; set; } = default!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;

        public Guid DoctorId { get; set; } = default!;

        public Doctor Doctor { get; set; } = default!;
    }

}

