
using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;


namespace ClincManagement.API.Services.Interface
{
    public interface IAppointmentService
    {
        Task<PagedAppointmentResponse> GetAppointmentByIdAsync(Guid appointmentId);
        Task<PagedAppointmentResponse> GetAllAppointmentsAsync();
        Task<PagedAppointmentResponse> CreateAppointmentAsync(CreateRequestAppointment request);
        Task<PagedAppointmentResponse> UpdateAppointmentAsync(Guid appointmentId, CreateRequestAppointment request);
        Task<bool> DeleteAppointmentAsync(Guid appointmentId);
     }
}
