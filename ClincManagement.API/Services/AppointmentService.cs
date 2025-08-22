using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;
using ClincManagement.API.Services.Interface;

namespace ClincManagement.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<PagedAppointmentResponse> CreateAppointmentAsync(CreateRequestAppointment request)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAppointmentAsync(Guid appointmentId)
        {

            // logic to delete appointment by Id
            if (appointmentId == Guid.Empty)
            {
                throw new ArgumentException("Appointment ID cannot be empty.", nameof(appointmentId));
            }
            var appointment =  _context.Appointments.FirstOrDefault(a => a.Id == appointmentId);
            if (appointment == null)
            {
                throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found.");
            }
            if (appointment.IsDeleted == true)
            {
                throw new InvalidOperationException($"Appointment with ID {appointmentId} is already deleted.");
            }
            appointment.IsDeleted = true; // Soft delete
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<PagedAppointmentResponse> GetAllAppointmentsAsync()
        {
            
            var appointments = await _context.Appointments
                .Where(a => a.IsDeleted== false)
                .Select(a => new ResponseDetailsAppointment(
                    a.Id,
                    a.Patient.User.FullName,
                    a.Doctor.User.FullName,
                    a.Clinic.Name,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Type,
                    a.Status))
                .ToListAsync();
            return await Task.FromResult(new PagedAppointmentResponse(appointments, appointments.Count, 1, appointments.Count, 1));

        }

        public async Task<PagedAppointmentResponse> GetAppointmentByIdAsync(Guid appointmentId)
        {
            if (appointmentId == Guid.Empty)
            {
                throw new ArgumentException("Appointment ID cannot be empty.", nameof(appointmentId));
            }
            var appointment = await _context.Appointments
                .Where(a => a.Id == appointmentId && a.IsDeleted == false)
                .Select(a => new ResponseDetailsAppointment(
                    a.Id,
                    a.Patient.User.FullName,
                    a.Doctor.User.FullName,
                    a.Clinic.Name,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Type,
                    a.Status))
                .FirstOrDefaultAsync();
            if (appointment == null)
            {
                throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found.");
            }
            return new PagedAppointmentResponse(new List<ResponseDetailsAppointment> { appointment }, 1, 1, 1, 1);


        }

        public async Task<PagedAppointmentResponse> UpdateAppointmentAsync(Guid appointmentId, CreateRequestAppointment request)
        {
            if (appointmentId == Guid.Empty)
            {
                throw new ArgumentException("Appointment ID cannot be empty.", nameof(appointmentId));
            }
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId && a.IsDeleted == false);
            if (appointment == null)
            {
                throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found.");
            }
            appointment.PatientId = request.PatientId;
            appointment.DoctorId = request.DoctorId;
            appointment.ClinicId = request.ClinicId;
            appointment.AppointmentDate = request.AppointmentDate;
            appointment.AppointmentTime = request.AppointmentTime;
            appointment.Duration = request.Duration;
            appointment.Type = request.appointmentType;
            appointment.Status = request.status ?? AppointmentStatus.Confirmed; 
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return await GetAppointmentByIdAsync(appointment.Id);

        }
    }
}
