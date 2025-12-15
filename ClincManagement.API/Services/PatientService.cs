using ClincManagement.API.Abstractions;

using ClincManagement.API.Contracts.Operation.Response;
using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Contracts.Patient.Responses;

using ClincManagement.API.Entities;
using ClincManagement.API.Errors;
using ClincManagement.API.Services.Interface;
using ClincManagement.API.Settings;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PatientResponseDto = ClincManagement.API.Contracts.Patient.Respones.PatientResponseDto;


public class PatientService : IPatientService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<PatientService> _logger;
    private readonly IImageFileService _imageFileService;

    public PatientService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<PatientService> logger,
        IImageFileService imageFileService)  
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
        _imageFileService = imageFileService;
    }

    //is Sucess
    public async Task<Result<PatientCreateResponseDto>> CreateAsync(
         string UserId,
         IFormFile? profileImage,
         PatientRequestDto request,
         CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber))
                    return Result.Failure<PatientCreateResponseDto>(UserErrors.DuplicatePhoneNumber);

                if (!string.IsNullOrEmpty(request.Email)
                    && await _userManager.FindByEmailAsync(request.Email) != null)
                    return Result.Failure<PatientCreateResponseDto>(UserErrors.DuplicatedEmail);

                if (!string.IsNullOrEmpty(request.NationalId)
                    && await _context.Patients.AnyAsync(p => p.NationalId == request.NationalId))
                    return Result.Failure<PatientCreateResponseDto>(PatientErrors.DuplicateNationalId);

                var user = new ApplicationUser
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    UserName = request.UserName,
                    PhoneNumber = request.PhoneNumber,
                    EmailConfirmed = true

                };

                var identityResult = await _userManager.CreateAsync(user);
                if (!identityResult.Succeeded)
                {
                    var err = identityResult.Errors.FirstOrDefault()?.Description;
                    return Result.Failure<PatientCreateResponseDto>(
                        new Error("User.CreateFailed", err ?? "Failed to create user"));
                }

                if (profileImage != null)
                {
                    var uploadedFile = await _imageFileService.UploadAsync(profileImage, "uploads/patients");
                    user.ProfileImage = uploadedFile;
                }

                var patient = new Patient
                {
                    PatientId = Guid.NewGuid(),
                    UserId = user.Id,
                    Gender = request.Gender,
                    SocialStatus = request.SocialStatus,
                    DateOfBirth = request.DateOfBirth,
                    NationalId = request.NationalId,
                    Address = request.Address,
                    Notes = request.Notes,
                    CreatedById = "System"
                };
                await _context.Patients.AddAsync(patient, cancellationToken);

                if (request.InitialBooking != null)
                {
                    var appointment = new Appointment
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patient.PatientId,
                        ClinicId = request.InitialBooking.ClinicId,
                        DoctorId = request.InitialBooking.DoctorId,
                        Type = request.InitialBooking.AppointmentType,
                        AppointmentDate = request.InitialBooking.AppointmentDate ?? DateTime.UtcNow,
                        Notes = request.InitialBooking.Notes,
                        CreatedById = "System",
                        UpdatedOn = DateTime.UtcNow,


                    };
                    await _context.Appointments.AddAsync(appointment, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Result.Success(new PatientCreateResponseDto(
                    patient.PatientId,
                    user.FullName,
                    user.PhoneNumber!,
                    user.Email,
                    patient.Address,
                    patient.Gender.ToString(),
                    patient.DateOfBirth,
                    patient.SocialStatus.ToString(),
                    patient.Notes,
                    user.ProfileImage?.FileName
                ));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                if (await _userManager.FindByNameAsync(request.UserName) is ApplicationUser createdUser)
                    await _userManager.DeleteAsync(createdUser);

                var inner = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "Error creating patient: {Error}", inner);

                return Result.Failure<PatientCreateResponseDto>(
                    new Error("Patient.Create.Error", inner));
            }
        });
    }

    //is Sucess
    public async Task<Result<PatientResponseDto>> GetPatientByIdAsync(Guid id)
    {
        var p = await _context.Patients
            .Include(x => x.User).ThenInclude(x => x.ProfileImage)
            .FirstOrDefaultAsync(x => x.PatientId == id);

        if (p == null) return Result.Failure<PatientResponseDto>(PatientErrors.NotFound);

        return Result.Success(
            new PatientResponseDto(
                p.PatientId,
                p.User.FullName,
                p.Gender,
                p.DateOfBirth,
                CalculateAge(p.DateOfBirth),
                p.SocialStatus,
                p.User.PhoneNumber!,
                p.User.Email!,
                p.NationalId,
                p.Address,
                p.Notes,
                p.User.ProfileImage?.StoredFileName,
                p.CreatedOn
            )
        );
    }

    public async Task<Result<PatientResponseDto?>> UpdatePatientAsync(Guid id, IFormFile? newProfileImage, PatientRequestDto request)
    {
        var p = await _context.Patients
            .Include(x => x.User)
            .ThenInclude(u => u.ProfileImage)
            .FirstOrDefaultAsync(x => x.PatientId == id);

        if (p == null)
            return Result.Failure<PatientResponseDto?>(PatientErrors.NotFound);

        if (p.User.PhoneNumber != request.PhoneNumber &&
            await _userManager.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber))
            return Result.Failure<PatientResponseDto?>(UserErrors.DuplicatePhoneNumber);

        if (p.User.Email != request.Email &&
            await _userManager.FindByEmailAsync(request.Email) != null)
            return Result.Failure<PatientResponseDto?>(UserErrors.DuplicatedEmail);

        if (p.NationalId != request.NationalId &&
            await _context.Patients.AnyAsync(x => x.NationalId == request.NationalId))
            return Result.Failure<PatientResponseDto?>(PatientErrors.DuplicateNationalId);


        // Update basic fields
        p.User.FullName = request.FullName;
        p.User.Email = request.Email;
        p.User.PhoneNumber = request.PhoneNumber;
        p.Gender = request.Gender;
        p.Address = request.Address;
        p.DateOfBirth = request.DateOfBirth;
        p.SocialStatus = request.SocialStatus;
        p.Notes = request.Notes;

        // Handle profile image update
        if (newProfileImage != null)
        {
            // Delete old image if exists
            if (p.User.ProfileImage != null)
            {
                _imageFileService.Delete(p.User.ProfileImage, "uploads/patients");
            }

            // Upload new one
            var uploadedFile = await _imageFileService.UploadAsync(newProfileImage, "uploads/patients");
            p.User.ProfileImage = uploadedFile;
        }

        await _context.SaveChangesAsync();

        return Result.Success<PatientResponseDto?>(
            new PatientResponseDto(
                p.PatientId,
                p.User.FullName,
                p.Gender,
                p.DateOfBirth,
                CalculateAge(p.DateOfBirth),
                p.SocialStatus,
                p.User.PhoneNumber!,
                p.User.Email!,
                p.NationalId,
                p.Address,
                p.Notes,
                p.User.ProfileImage?.StoredFileName,
                p.CreatedOn
            )
        );
    }
    //is sucess
    public async Task<Result> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var p = await _context.Patients
            .Include(x => x.User)
            .ThenInclude(u => u.ProfileImage)
            .FirstOrDefaultAsync(x => x.PatientId == id, cancellationToken);

        if (p == null)
            return Result.Failure(PatientErrors.NotFound);

      
        if (p.User?.ProfileImage != null)
        {
            _imageFileService.Delete(p.User.ProfileImage, "uploads/patients");
        }

      
        p.IsDeleted = true;
        p.DeletedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(new { Message = "Patient deleted successfully" });
    }


    //is sucsses
    public async Task<Result<PagedPatientResponse>> GetPatientsAsync(
      string? search,
      int page = 1,
      int pageSize = 10)
    {
        IQueryable<Patient> q = _context.Patients
            .Include(x => x.User);

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();

            q = q.Where(x =>
                x.User.FullName.ToLower().Contains(search) ||
                x.User.PhoneNumber.Contains(search) ||
                x.NationalId.Contains(search)
            );
        }

        int total = await q.CountAsync();

        var data = await q
            .OrderBy(x => x.User.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var list = data.Select(x => new PatientListResponseDto(
            x.PatientId,
            x.User.FullName,
            x.Gender,
            CalculateAge(x.DateOfBirth),
            x.User.PhoneNumber,
            x.Address
        ));

        return Result.Success(
            new PagedPatientResponse(
                list,
                total,
                page,
                pageSize,
                (int)Math.Ceiling(total / (double)pageSize)
            )
        );
    }



    //public async Task<Result<IEnumerable<ResponsePatientOperation>>> GetAllOperationsByPatientIdAsync(Guid patientId)
    //{
    //    if (await GetPatientByIdAsync(patientId) is null)
    //        return Result.Failure<IEnumerable<ResponsePatientOperation>>(PatientErrors.NotFound);

    //    var ops = await _context.Operations
    //        .AsNoTracking()
    //        .Include(o => o.Doctor)
    //        .Where(o => o.PatientId == patientId)
    //        .OrderByDescending(o => o.Date)
    //        .ToListAsync();

    //    var mapped = ops.Select(o => new ResponsePatientOperation(
    //        o.Name,
    //        o.Date.ToString("yyyy-MM-dd"),
    //        o.Doctor?.FullName ?? "Unknown",
    //        o.Tools,
    //        $"{o.Cost:C} - {o.Notes}"

    //    ));

    //    return Result.Success(mapped);
    //}

    public async Task<Result<IEnumerable<ResponsePatientInvoice>>> GetAllInvoicesByPatientIdAsync(Guid patientId)
    {
        // Check if patient exists
        if (await GetPatientByIdAsync(patientId) is null)
            return Result.Failure<IEnumerable<ResponsePatientInvoice>>(PatientErrors.NotFound);

        // Fetch invoices for the patient
        var invoices = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.PatientId == patientId)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();

        var mapped = invoices.Select(i => new ResponsePatientInvoice
        {
            InvoiceNumber = i.InvoiceNumber ?? "N/A",
            InvoiceDate = i.InvoiceDate.ToString("yyyy-MM-dd"),
            FinalAmount = i.FinalAmountEGP,
            PaidAmount = i.PaidAmountEGP,
            RemainingAmount = i.FinalAmountEGP - i.PaidAmountEGP,
            Status = i.Status.ToString()
        });



        return Result.Success(mapped);
    }



    public async Task<Result<IEnumerable<ResponseAllAppointmentPatient>>> GetAllAppointmentsByPatientIdAsync(Guid patientId)
    {
        if (await GetPatientByIdAsync(patientId) is null)
            return Result.Failure<IEnumerable<ResponseAllAppointmentPatient>>(PatientErrors.NotFound);

        var appointments = await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.Invoice)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        if (!appointments.Any())
            return Result.Success(Enumerable.Empty<ResponseAllAppointmentPatient>());

        var mapped = appointments.Select(a => new ResponseAllAppointmentPatient
        {
            AppointmentId = a.Id,
            DoctorName = a.Doctor?.FullName ?? "Unknown",
            Specialization = a.Doctor?.Specialization ?? "N/A",
            Date = a.AppointmentDate,
            Time = a.AppointmentTime,
            VisitType = a.Type.ToString(),
            Status = a.Status.ToString(),
            PaymentStatus = a.Invoice?.Status.ToString() ?? "N/A",
          
                
        });

        return Result.Success(mapped);
    }




    public async Task<Result<IEnumerable<ResponsePatientStay>>> GetAllStaysByPatientIdAsync(Guid patientId)
    {
        if (await GetPatientByIdAsync(patientId) is null)
            return Result.Failure<IEnumerable<ResponsePatientStay>>(PatientErrors.NotFound);

        var stays = await _context.Stays
            .AsNoTracking()
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync();

        var mapped = stays.Select(s => new ResponsePatientStay
        {
            RoomBed = $"{s.RoomNumber}/{s.BedNumber}",
            CheckInDate = s.StartDate.ToString("yyyy-MM-dd HH:mm"),
            CheckOutDate = s.EndDate?.ToString("yyyy-MM-dd HH:mm") ?? "N/A",
            
        });


        return Result.Success(mapped);
    }

    private static int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        int age = today.Year - dob.Year;
        if (dob > today.AddYears(-age)) age--;
        return age;
    }

    

   
}
