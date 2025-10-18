using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;
using ClincManagement.API.Contracts.Appointments.Responses;
using ClincManagement.API.Errors.ClincManagement.API.Errors;
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


        public async Task<Result<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto request, CancellationToken cancel)
        {

            var patient = await _context.Patients.FindAsync(request.PatientId);
            var doctor = await _context.Doctors
                .Include(d => d.Clinic)
                .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancel);

            if (patient == null) return Result.Failure<AppointmentDto>(AppointmentErrors.InvalidPatient);
            if (doctor == null) return Result.Failure<AppointmentDto>(AppointmentErrors.InvalidDoctor);
            if (request.Date < DateTime.UtcNow) return Result.Failure<AppointmentDto>(AppointmentErrors.InvalidDate);

            var conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == request.DoctorId &&
                a.AppointmentDate == request.Date &&
                a.AppointmentTime == request.Time &&
                !a.IsDeleted, cancel);

            if (conflict) return Result.Failure<AppointmentDto>(AppointmentErrors.AlreadyExists);

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patient.PatientId,
                DoctorId = doctor.Id,
                ClinicId = doctor.ClinicId,
                AppointmentDate = request.Date,
                AppointmentTime = request.Time,
                Duration = request.DurationMinutes,
                Type = request.AppointmentType,
                Status = request.Status ?? AppointmentStatus.Waiting,
                Notes = request.Notes,
                UpdatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancel);

            var dto = new AppointmentDto
            {
                AppointmentId = appointment.Id,
                PatientName = patient.User.FullName,
                DoctorName = doctor.FullName,
                ClinicName = doctor.Clinic.Name,
                AppointmentType = appointment.Type.ToString(),
                Status = appointment.Status.ToString(),
                Date = appointment.AppointmentDate,
                Time = appointment.AppointmentTime,
                DurationMinutes = appointment.Duration,
                Notes = appointment.Notes
            };

            return Result.Success(dto);
        }

        // Update Appointment
        public async Task<Result> UpdateAppointmentAsync(UpdateAppointmentDto request, CancellationToken cancel)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId && !a.IsDeleted, cancel);

            if (appointment == null) return Result.Failure(AppointmentErrors.NotFound);

            var patientExists = await _context.Patients.AnyAsync(p => p.PatientId == request.PatientId, cancel);
            if (!patientExists) return Result.Failure(AppointmentErrors.InvalidPatient);

            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == request.DoctorId, cancel);
            if (!doctorExists) return Result.Failure(AppointmentErrors.InvalidDoctor);

            if (request.Date < DateTime.UtcNow) return Result.Failure(AppointmentErrors.InvalidDate);

            var conflict = await _context.Appointments.AnyAsync(a =>
                a.Id != request.AppointmentId &&
                a.DoctorId == request.DoctorId &&
                a.AppointmentDate == request.Date &&
                a.AppointmentTime == request.Time &&
                !a.IsDeleted, cancel);

            if (conflict) return Result.Failure(AppointmentErrors.AlreadyExists);

            appointment.PatientId = request.PatientId;
            appointment.DoctorId = request.DoctorId;
            appointment.ClinicId = request.ClinicId;
            appointment.AppointmentDate = request.Date;
            appointment.AppointmentTime = request.Time;
            appointment.Duration = request.DurationMinutes;
            appointment.Type = request.AppointmentType;
            appointment.Status = request.Status ?? AppointmentStatus.Waiting;
            appointment.Notes = request.Notes;
            appointment.UpdatedDate = DateTime.UtcNow;

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync(cancel);

            return Result.Success();
        }


        public async Task<Result> DeleteAppointmentAsync(Guid appointmentId, CancellationToken cancel)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null) return Result.Failure(AppointmentErrors.NotFound);

            appointment.IsDeleted = true;
            appointment.UpdatedDate = DateTime.UtcNow;

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync(cancel);

            return Result.Success();
        }


        public async Task<Result<PagedAppointmentResponse>> GetAllAppointmentsAsync(int page, int pageSize, CancellationToken cancel)
        {
            var query = _context.Appointments
                .Where(a => !a.IsDeleted)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Clinic);

            var totalCount = await query.CountAsync(cancel);
            if (totalCount == 0) return Result.Failure<PagedAppointmentResponse>(AppointmentErrors.NotFound);

            var appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancel);

            var data = appointments.Select(a => new ResponseDetailsAllAppointment(
                AppointmentId: a.Id,
                PatientName: a.Patient.User.FullName,
                DoctorName: a.Doctor.FullName,
                Clinic: a.Doctor.Clinic.Name,
                AppointmentDate: a.AppointmentDate,
                AppointmentTime: a.AppointmentTime,
                AppointmentType: a.Type,
                Status: a.Status
            )).ToList();

            var response = new PagedAppointmentResponse(
                Data: data,
                TotalCount: totalCount,
                Page: page,
                PageSize: pageSize,
                TotalPages: (int)Math.Ceiling(totalCount / (double)pageSize)
            );

            return Result.Success(response);
        }


        public async Task<Result<AppointmentDetailsResponse>> GetAppointmentsByPatientIdAsync(Guid patientId, CancellationToken cancel)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Clinic)
                .FirstOrDefaultAsync(a => a.PatientId == patientId && !a.IsDeleted, cancel);

            if (appointment == null) return Result.Failure<AppointmentDetailsResponse>(AppointmentErrors.NotFound);

            var response = new AppointmentDetailsResponse
            {
                AppointmentId = appointment.Id.ToString(),
                Status = appointment.Status.ToString(),
                ReferenceCode = "",
                InvoiceId = "",
                PaymentStatus = "",
                DoctorDetails = new DoctorDetails
                {
                    Name = appointment.Doctor.FullName,
                    ClinicName = appointment.Doctor.Clinic.Name
                },
                BookingTime = new BookingTime
                {
                    Date = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                    Time = appointment.AppointmentTime
                },
                FinancialSummary = new FinancialSummary
                {
                    ConsultationFees = 0,
                    Discount = 0,
                    TotalPaid = 0
                },
                Notes = appointment.Notes
            };

            return Result.Success(response);
        }


        public async Task<Result<ResponserAppointmentDto>> CreateAppointmentPatientAsync(BookAppointmentRequest request, Guid patientId, CancellationToken cancel)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Clinic)
                .FirstOrDefaultAsync(d => d.Id == Guid.Parse(request.DoctorId), cancel);

            if (doctor == null) return Result.Failure<ResponserAppointmentDto>(AppointmentErrors.InvalidDoctor);
            if (request.Date < DateTime.UtcNow) return Result.Failure<ResponserAppointmentDto>(AppointmentErrors.InvalidDate);

            var conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctor.Id &&
                a.AppointmentDate == request.Date &&
                a.AppointmentTime == request.Time &&
                !a.IsDeleted, cancel);

            if (conflict) return Result.Failure<ResponserAppointmentDto>(AppointmentErrors.AlreadyExists);

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                DoctorId = doctor.Id,
                ClinicId = doctor.ClinicId,
                AppointmentDate = request.Date,
                AppointmentTime = request.Time,
                Duration = 30,
                Type = request.ConsultationType == "Video_Consultation" ? AppointmentType.First_Visit : AppointmentType.Initial_Exam,
                Status = AppointmentStatus.Waiting,
                Notes = request.ReasonForVisit,
                UpdatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancel);

            return Result.Success(new ResponserAppointmentDto
            {
                AppointmentId = appointment.Id.GetHashCode(),
                Massage = "Appointment Created Successfully"
            });
        }
    }
}
