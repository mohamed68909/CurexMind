namespace ClincManagement.API.Entities
{
    public class Clinic
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string Name { get; set; } =  string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
         public bool? IsActive { get; set; }=true;
        public DateTime CreatedDate { get; set; }=DateTime.Now;
        public ICollection<Appointment> Appointments { get; set; } = default!;





    }
}
