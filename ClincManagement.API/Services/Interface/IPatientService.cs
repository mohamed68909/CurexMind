using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Contracts.Patient.Responses;

namespace ClincManagement.API.Services.Interface
{

    public interface IPatientService
    {
        Task<PatientResponseDto?> GetPatientByIdAsync(Guid id);
        Task<PagedPatientResponse> GetPatientsAsync(string? search,int page = 1, int pageSize = 10);
        Task<PatientCreateResponseDto> CreatePatientAsync(PatientRequestDto request);
        Task<PatientResponseDto?> UpdatePatientAsync(Guid id, PatientRequestDto request);
        Task<bool> DeletePatientAsync(Guid id);
    
}

}
