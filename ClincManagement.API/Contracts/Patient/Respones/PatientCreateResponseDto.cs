namespace ClincManagement.API.Contracts.Patient.Respones
{
    public record PatientCreateResponseDto
    (
        Guid PatientId,
        string FullName,
        string PhoneNumber,
        string Email,
        string Address,
        string Gender,
        DateTime DateOfBirth,
        string SocialStatus,
        string Notes,

        string? ProfileImageUrl = null

    );
}
