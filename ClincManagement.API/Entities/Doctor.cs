namespace ClincManagement.API.Entities
{
    public sealed class Doctor
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string Specialization { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;


        public string FullName { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
        public Clinic Clinic { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;
        public ICollection<Operation> Operations { get; set; } = default!;
        public ICollection<Appointment> Appointments { get; set; } = default!;

        public  ICollection<Review> Reviews { get; set; } = default !;

    }
}
