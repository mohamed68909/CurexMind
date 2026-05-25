
namespace ClinicManagement.API.Contracts.Patient.Responses
{
    public record PatientListResponseDto
    (
        Guid PatientId,

        string FullName,
        Gender Gender,
        int Age,
        string? PhoneNumber,
        string? Address

    )
    {


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
