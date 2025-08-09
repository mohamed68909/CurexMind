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

    }
}
