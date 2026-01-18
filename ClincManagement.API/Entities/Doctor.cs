using ClincManagement.API.Settings;

public sealed class Doctor : Auditable
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FullName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string Languages { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string bio {  get; set; } = string.Empty;
    public Guid ClinicId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public Clinic Clinic { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    public ICollection<Operation> Operations { get; set; } = new List<Operation>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
