using ClincManagement.API.Contracts.Clinic.Respones;
using ClincManagement.API.Services.Interface;

namespace ClincManagement.API.Services
{
    public class DoctorService : IDoctorService
    {
        readonly ApplicationDbContext _context;
        public DoctorService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ResponseDetailsDoctors>> GetAll()
        {
            var doctors = await _context.Doctors
                .Select(d => new ResponseDetailsDoctors
                (
                    d.Id,
                    d.FullName,
                    d.Specialization
                )).ToListAsync();
            return doctors;

        }
    }
}
