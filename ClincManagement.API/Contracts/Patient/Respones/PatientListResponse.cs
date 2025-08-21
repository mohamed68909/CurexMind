
namespace ClincManagement.API.Contracts.Patient.Responses
{
    public record PatientListResponseDto
    (
        Guid PatientId,
       
        string FullName,
        Gender Gender,
        int Age,
        string? PhoneNumber,
        string? Address,
        DateTime RegistrationDate
    )
    {
        private Guid id;
        private Gender gender;

      
    }

    public record PagedPatientResponse
    (
        IEnumerable<PatientListResponseDto> Data,
        int TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    );
}
