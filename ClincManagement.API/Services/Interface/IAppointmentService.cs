
using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;


namespace ClincManagement.API.Services.Interface
{
    public interface IAppointmentService
    {

        Task<Result<PagedAppointmentResponse>> GetAllAppointmentsAsync(int page, int pageSize, CancellationToken cancel);

        //Task<AppointmentDto?> GetAppointmentByIdAsync(Guid appointmentId, CancellationToken cancel);


        Task<Result<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto request, CancellationToken cancel);

    
        Task<Result> UpdateAppointmentAsync(UpdateAppointmentDto request, CancellationToken cancel);

        
        Task<Result> DeleteAppointmentAsync(Guid appointmentId, CancellationToken cancel);
    }
}
