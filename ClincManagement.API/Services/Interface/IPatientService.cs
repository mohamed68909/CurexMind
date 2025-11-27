
using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Invoice.Respones;
using ClincManagement.API.Contracts.Operation.Response;
using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Contracts.Patient.Responses;

using PatientResponseDto = ClincManagement.API.Contracts.Patient.Respones.PatientResponseDto;

namespace ClincManagement.API.Services.Interface
{

    public interface IPatientService
    {

        Task<Result<PatientResponseDto>> GetPatientByIdAsync(Guid id);

        Task<Result<PagedPatientResponse>> GetPatientsAsync(string? search, int page = 1, int pageSize = 10);


        Task<Result<PatientCreateResponseDto>> CreateAsync(string UserId , IFormFile? profileImage, PatientRequestDto request , CancellationToken cancellationToken = default);


        Task<Result<PatientResponseDto?>> UpdatePatientAsync(Guid id, IFormFile? profileImage, PatientRequestDto request);


        //Task<Result<IEnumerable<ResponseAllAppointmentPatient>>> GetAllAppointmentsByPatientIdAsync(Guid patientId);


        //Task<Result<IEnumerable<ResponsePatientInvoice>>> GetAllInvoicesByPatientIdAsync(Guid patientId);


        //Task<Result<IEnumerable<ResponsePatientStay>>> GetAllStaysByPatientIdAsync(Guid patientId);


        //Task<Result<IEnumerable<ResponsePatientOperation>>> GetAllOperationsByPatientIdAsync(Guid patientId);


        Task<Result> Delete(Guid id, CancellationToken cancellationToken = default);


    }
}


