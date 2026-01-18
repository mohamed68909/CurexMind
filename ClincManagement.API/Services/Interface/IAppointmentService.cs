using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;
using ClincManagement.API.Contracts.Appointments.Responses;
using ClincManagement.API.Contracts.Appointments.Responses.ClincManagement.API.Contracts.Appointments.Responses;

namespace ClincManagement.API.Services.Interface
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
