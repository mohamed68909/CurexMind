namespace ClincManagement.API.Entities
{
    public class ApplicationUser : IdentityUser
    {
       
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public Doctor Doctor { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Patient Patient { get; set; } = default!;
        public ICollection<RefreshToken> refreshTokens { get; set; } = default!;
        public bool IsDisabled { get; internal set; }
        public ICollection<VitalSigns> VitalSigns { get; set; } = default!;
      
    }
}
