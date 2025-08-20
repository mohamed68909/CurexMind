using ClincManagement.API.Contracts.Clinic.Respones;

namespace ClincManagement.API.Services.Interface
{
    public interface IDoctorService
    {
        Task<IEnumerable<ResponseDetailsDoctors>> GetAll();

    }
}
