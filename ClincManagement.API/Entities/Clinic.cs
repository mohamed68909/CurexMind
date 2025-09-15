namespace ClincManagement.API.Entities
{
    public class Clinic
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string Name { get; set; } =  string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
         public bool? IsActive { get; set; }=true;
        public DateTime CreatedDate { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = default!;

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

      
        public ICollection<Review> Reviews { get; set; } = new List<Review>();




    }
}
