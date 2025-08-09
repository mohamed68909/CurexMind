namespace ClincManagement.API.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public Doctor Doctor { get; set; } = default!;
        public Patient Patient { get; set; } = default!;


    }
}
