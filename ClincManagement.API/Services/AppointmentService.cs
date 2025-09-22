using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;
using ClincManagement.API.Errors;
using ClincManagement.API.Errors.ClincManagement.API.Errors;
using ClincManagement.API.Services.Interface;
using Mapster;
using System.Threading;

namespace ClincManagement.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto request, CancellationToken cancel)
        {
            
            var patientExists = await _context.Patients.AnyAsync(p => p.PatientId == request.PatientId, cancel);
            if (!patientExists)
                return Result.Failure<AppointmentDto>(AppointmentErrors.InvalidPatient);

            
            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == request.DoctorId, cancel);
            if (!doctorExists)
                return Result.Failure<AppointmentDto>(AppointmentErrors.InvalidDoctor);

            
            if (request.Date < DateTime.UtcNow)
                return Result.Failure<AppointmentDto>(AppointmentErrors.InvalidDate);

            
            var isConflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == request.DoctorId &&
                a.AppointmentDate == request.Date &&
                a.AppointmentTime == request.Time &&
                a.IsDeleted == false, cancel);

            if (isConflict)
                return Result.Failure<AppointmentDto>(AppointmentErrors.AlreadyExists);

            try
            {
                
                var appointment = request.Adapt<Appointment>();
                appointment.Id = Guid.CreateVersion7();
                appointment.UpdatedDate = DateTime.UtcNow;
                appointment.IsDeleted = false;

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync(cancel);

               
                var dto = appointment.Adapt<AppointmentDto>();
                return Result.Success(dto);
            }
            catch
            {
                return Result.Failure<AppointmentDto>(AppointmentErrors.CreationFailed);
            }
        }


        public async Task<Result> DeleteAppointmentAsync(Guid appointmentId, CancellationToken cancel)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId, cancel);

            if (appointment is null)
                return Result.Failure(AppointmentErrors.NotFound);

            
            appointment.IsDeleted = true;
            appointment.UpdatedDate = DateTime.UtcNow; 

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync(cancel);

            return Result.Success();
        }


    

        public async Task<Result> UpdateAppointmentAsync(UpdateAppointmentDto request, CancellationToken cancel)
        {
          
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId && a.IsDeleted == false, cancel);

            if (appointment is null)
                return Result.Failure(AppointmentErrors.NotFound);

        
            var patientExists = await _context.Patients.AnyAsync(p => p.PatientId == request.PatientId, cancel);
            if (!patientExists)
                return Result.Failure(AppointmentErrors.InvalidPatient);

          
            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == request.DoctorId, cancel);
            if (!doctorExists)
                return Result.Failure(AppointmentErrors.InvalidDoctor);

       
            if (request.Date < DateTime.UtcNow)
                return Result.Failure(AppointmentErrors.InvalidDate);

           
            var isConflict = await _context.Appointments.AnyAsync(a =>
                a.Id != request.PatientId &&
                a.DoctorId == request.DoctorId &&
                a.AppointmentDate == request.Date &&
                a.AppointmentTime == request.Time &&
                a.IsDeleted == false, cancel);

            if (isConflict)
                return Result.Failure(AppointmentErrors.AlreadyExists);

            try
            {
                
                appointment.PatientId = request.PatientId;
                appointment.DoctorId = request.DoctorId;
                appointment.ClinicId = request.ClinicId;
                appointment.AppointmentDate = request.Date;
                appointment.AppointmentTime = request.Time;
                appointment.Notes = request.Notes;
                appointment.Duration = request.DurationMinutes;
                appointment.Type = request.AppointmentType;
                appointment.Status = request.Status.GetValueOrDefault();
                appointment.UpdatedDate = DateTime.UtcNow;

                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync(cancel);

                return Result.Success();
            }
            catch
            {
                return Result.Failure(AppointmentErrors.UpdateFailed);
            }
        }

      public async Task<Result<PagedAppointmentResponse>> GetAllAppointmentsAsync( int page, int pageSize, CancellationToken cancel)
{
   
    var query = _context.Appointments
        .Where(a => a.IsDeleted == false)
        .Include(a => a.Patient)
        .Include(a => a.Doctor)
        .Include(a => a.Clinic);


    var totalCount = await query.CountAsync(cancel);

    if (totalCount == 0)
        return Result.Failure<PagedAppointmentResponse>(AppointmentErrors.NotFound);

    
    var appointments = await query
        .OrderBy(a => a.AppointmentDate)
        .ThenBy(a => a.AppointmentTime)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancel);

  
    var data = appointments.Adapt<IEnumerable<ResponseDetailsAppointment>>();

    
    var response = new PagedAppointmentResponse(
        Data: data,
        TotalCount: totalCount,
        Page: page,
        PageSize: pageSize,
        TotalPages: (int)Math.Ceiling(totalCount / (double)pageSize)
    );

    return Result.Success(response);
}

    }
}
