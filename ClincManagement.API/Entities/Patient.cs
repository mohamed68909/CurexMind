namespace ClincManagement.API.Entities
{
    public sealed class Patient
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string NationalId { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.Other;

        public SocialStatus SocialStatus { get; set; } = SocialStatus.Single;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Invoice> Invoice { get; set; } = new List<Invoice>();

        public ICollection<Stay> Stays { get; set; } = new List<Stay>();
        public ICollection<Operation> Operations { get; set; } = new List<Operation>();



    }
}
