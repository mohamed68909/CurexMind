using CurexMind.API.Abstractions;
using CurexMind.API.Contracts.Appinments.Requests;
using CurexMind.API.Contracts.Appinments.Respones;
using CurexMind.API.Contracts.Appointments.Responses;
using CurexMind.API.Contracts.Appointments.Responses.CurexMind.API.Contracts.Appointments.Responses;

namespace CurexMind.API.Services.Interface
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
