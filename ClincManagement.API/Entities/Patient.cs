using ClincManagement.API.Settings;

namespace ClincManagement.API.Entities
{
    public sealed class Patient : Auditable
    {
        public Guid PatientId { get; set; } = Guid.CreateVersion7();
        public string NationalId { get; set; } = string.Empty;

        public Gender Gender { get; set; } = Gender.Other;
        public string? Address { get; set; }
        public string? Notes { get; set; }

        public DateTime DateOfBirth { get; set; }
        public SocialStatus SocialStatus { get; set; } = SocialStatus.Single;

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Navigation Collections
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public ICollection<Stay> Stays { get; set; } = new List<Stay>();
        public ICollection<Operation> Operations { get; set; } = new List<Operation>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
