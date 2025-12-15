using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;

using ClincManagement.API.Contracts.Appointments.Responses;
using ClincManagement.API.Errors;
using ClincManagement.API.Services.Interface;
using Microsoft.EntityFrameworkCore;

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
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == request.PatientId, cancel);

            var doctor = await _context.Doctors
                .Include(d => d.Clinic)
                .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancel);

            if (patient == null) return Result.Failure<AppointmentDto>(AppointmentErrors.InvalidPatient);
            if (doctor == null) return Result.Failure<AppointmentDto>(AppointmentErrors.InvalidDoctor);
            if (request.Date < DateTime.UtcNow.Date) return Result.Failure<AppointmentDto>(AppointmentErrors.InvalidDate);

            var conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == request.DoctorId &&
                a.AppointmentDate == request.Date.Date &&
                a.AppointmentTime == request.Time &&
                !a.IsDeleted, cancel);

            if (conflict) return Result.Failure<AppointmentDto>(AppointmentErrors.AlreadyExists);

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patient.PatientId,
                DoctorId = doctor.Id,
                ClinicId = doctor.ClinicId,
                AppointmentDate = request.Date.Date,
                AppointmentTime = request.Time,
                Duration = request.DurationMinutes,
                Type = request.AppointmentType,
                Status = request.Status ?? AppointmentStatus.Waiting,
                Notes = request.Notes,
                UpdatedOn = DateTime.UtcNow,
                IsDeleted = false,
                CreatedById = request.PatientId.ToString()
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancel);

            var dto = new AppointmentDto
            {
                AppointmentId = appointment.Id,
                PatientName = patient.User?.FullName ?? "No Patient",
                DoctorName = doctor.FullName ?? "No Doctor",
                ClinicName = doctor.Clinic?.Name ?? "No Clinic",
                AppointmentType = appointment.Type.ToString(),
                Status = appointment.Status.ToString(),
                Date = appointment.AppointmentDate,
                Time = appointment.AppointmentTime.ToString(@"hh\:mm"),
                DurationMinutes = appointment.Duration,
                Notes = appointment.Notes,
            };

            return Result.Success(dto);
        }

        public async Task<Result> UpdateAppointmentAsync(UpdateAppointmentDto request, CancellationToken cancel)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId && !a.IsDeleted, cancel);

            if (appointment == null) return Result.Failure(AppointmentErrors.NotFound);

            appointment.PatientId = request.PatientId;
            appointment.DoctorId = request.DoctorId;
            appointment.ClinicId = request.ClinicId;
            appointment.AppointmentDate = request.Date.Date;
            appointment.AppointmentTime = request.Time;
            appointment.Duration = request.DurationMinutes;
            appointment.Type = request.AppointmentType;
            appointment.Status = request.Status ?? AppointmentStatus.Waiting;
            appointment.Notes = request.Notes;
            appointment.UpdatedOn = DateTime.UtcNow;

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync(cancel);

            return Result.Success();
        }

        public async Task<Result> DeleteAppointmentAsync(Guid appointmentId, CancellationToken cancel)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null) return Result.Failure(AppointmentErrors.NotFound);

            appointment.IsDeleted = true;
            appointment.UpdatedOn = DateTime.UtcNow;

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync(cancel);

            return Result.Success();
        }

        public async Task<Result<PagedAppointmentResponse>> GetAllAppointmentsAsync(int page, int pageSize, CancellationToken cancel)
        {
            var query = _context.Appointments
                .Where(a => !a.IsDeleted)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
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
                PatientName: a.Patient?.User?.FullName ?? "No Patient",
                DoctorName: a.Doctor?.FullName ?? "No Doctor",
                Clinic: a.Doctor?.Clinic?.Name ?? "No Clinic",
                AppointmentDate: a.AppointmentDate,
                AppointmentTime: a.AppointmentTime.ToString(@"hh\:mm"),
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
                    Name = appointment.Doctor?.FullName ?? "No Doctor",
                    ClinicName = appointment.Doctor?.Clinic?.Name ?? "No Clinic"
                },
                BookingTime = new BookingTime
                {
                    Date = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                    Time = appointment.AppointmentTime.ToString(@"hh\:mm")
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
            if (request.Date < DateTime.UtcNow.Date) return Result.Failure<ResponserAppointmentDto>(AppointmentErrors.InvalidDate);

            var conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctor.Id &&
                a.AppointmentDate == request.Date.Date &&
                a.AppointmentTime == request.Time &&
                !a.IsDeleted, cancel);

            if (conflict) return Result.Failure<ResponserAppointmentDto>(AppointmentErrors.AlreadyExists);

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                DoctorId = doctor.Id,
                ClinicId = doctor.ClinicId,
                AppointmentDate = request.Date.Date,
                AppointmentTime = request.Time,
                Duration = 30,
                Type = request.ConsultationType == "Video_Consultation" ? AppointmentType.First_Visit : AppointmentType.Initial_Exam,
                Status = AppointmentStatus.Waiting,
                Notes = request.ReasonForVisit,
                UpdatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancel);

            return Result.Success(new ResponserAppointmentDto
            {
                AppointmentId = appointment.Id,
           
            });
        }
    }
}
