namespace ClincManagement.API.Contracts.Patient.Respones
{
    public record PatientResponseDto
    (
        Guid PatientId,
        string FullName,
        Gender Gender,
        DateTime DateOfBirth,
        int Age,
        SocialStatus SocialStatus,
        string PhoneNumber,
        string Email,
        string NationalId,
        string Address,
        string Notes,
        string? ProfileImageUrl,

        DateTime CreatedDate
    );



}
