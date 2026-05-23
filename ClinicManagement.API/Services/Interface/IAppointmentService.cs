using ClinicManagement.API.Abstractions;
using ClinicManagement.API.Contracts.Appinments.Requests;
using ClinicManagement.API.Contracts.Appinments.Respones;
using ClinicManagement.API.Contracts.Appointments.Responses;
using ClinicManagement.API.Contracts.Appointments.Responses.ClinicManagement.API.Contracts.Appointments.Responses;

namespace ClinicManagement.API.Services.Interface
{
    public interface IAppointmentService
    {
        Task<Result<PagedAppointmentResponse>> GetAllAppointmentsAsync(int page, int pageSize, CancellationToken cancel);

        Task<Result<AppointmentDetailsResponse>> GetAppointmentsByIdAsync(Guid appointmentId, CancellationToken cancel);

        Task<Result<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto request, CancellationToken cancel);

        Task<Result<ResponserAppointmentDto>> CreateAppointmentPatientAsync(BookAppointmentRequest request, Guid patientId, CancellationToken cancel);

        Task<Result> UpdateAppointmentAsync(UpdateAppointmentDto request, CancellationToken cancel);

        Task<Result> DeleteAppointmentAsync(Guid appointmentId, CancellationToken cancel);
        Task<Result<List<MyAppointmentResponse>>> GetMyAppointmentsAsync(Guid patientId, AppointmentFilter filter, CancellationToken cancel);
    }
}
