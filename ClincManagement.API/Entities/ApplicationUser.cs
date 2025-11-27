namespace ClincManagement.API.Entities
{
    public class ApplicationUser : IdentityUser
    {

        public string FullName { get; set; } = string.Empty;
        public UploadedFile? ProfileImage { get; set; } 
        public Doctor? Doctor { get; set; } 
        public Patient? Patient { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public bool IsDisabled { get; internal set; }


    }
}
