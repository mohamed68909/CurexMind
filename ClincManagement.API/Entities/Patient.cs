namespace ClincManagement.API.Entities
{
    public sealed class Patient 
    {
        public Guid PatientId { get; set; } = Guid.CreateVersion7();
        public string NationalId { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.Other;
     
        public string Notes { get; set; }
            = string.Empty;
        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } 
        public DateTime UpdatedDate { get; set; } 
        public bool IsActive { get; set; } = true;


        public SocialStatus SocialStatus { get; set; } = SocialStatus.Single;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;
        public ICollection<Appointment> Appointments { get; set; } = default!;
        public ICollection<Invoice> Invoice { get; set; } = default!;

        public ICollection<Stay> Stays { get; set; } = default!;
        public ICollection<Operation> Operations { get; set; } = default!;
        

        public ICollection<Review> Reviews { get; set; } = default!;


    }
}
