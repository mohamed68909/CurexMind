
using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Contracts.Patient.Responses;

namespace ClincManagement.API.Services.Interface
{

    public interface IPatientService
    {
        // Task<Result<PatientResponseDto>> GetPatientByIdAsync(Guid id);
        //Task<Result<PagedPatientResponse>> GetPatientsAsync(string? search,int page = 1, int pageSize = 10);
        Task<Result<PatientCreateResponseDto>> CreateAsync(string UserId, PatientRequestDto request, CancellationToken cancellationToken = default);
        //Task<Result<PatientResponseDto?>> UpdatePatientAsync(Guid id, PatientRequestDto request);
        Task<Result<IEnumerable<ResponseAllAppointmentPatient>>> GetAllAppointmentsByPatientIdAsync(Guid patientId);
        Task <Result> Delete(Guid id, CancellationToken cancellationToken = default);

    
}

}
