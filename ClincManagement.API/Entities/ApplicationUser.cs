namespace ClincManagement.API.Entities
{
    public class ApplicationUser : IdentityUser
    {
       
        public string FullName { get; set; }
     
        public string Address { get; set; }
        public Doctor Doctor { get; set; } = default!;
        public Patient Patient { get; set; } = default!;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public bool IsDisabled { get; internal set; }
        public ICollection<VitalSigns> VitalSigns { get; set; } = default!;
      
    }
}
