using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Invoice.Respones;

using ClincManagement.API.Contracts.Operation.Response;

using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Contracts.Patient.Responses;
using ClincManagement.API.Contracts.Stay.Responses;
using ClincManagement.API.Errors;
using ClincManagement.API.Services.Interface;
using Mapster;
using MapsterMapper;
using Error = ClincManagement.API.Abstractions.Error;

public class PatientService : IPatientService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
    ApplicationDbContext context,
    IMapper mapper,
    UserManager<ApplicationUser> userManager,
    ILogger<PatientService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _logger = logger;
    }

    public async Task<Result<PatientCreateResponseDto>> CreateAsync(
    string userId,
    PatientRequestDto request,
    CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting patient creation for Phone: {Phone}, Email: {Email}", request.PhoneNumber, request.Email);

        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);

        if (existingUser is not null)
            return Result.Failure<PatientCreateResponseDto>(UserErrors.DuplicatePhoneNumber);

        if (!string.IsNullOrEmpty(request.NationalId))
        {
            var nationalIdExists = await _context.Patients
                .AnyAsync(p => p.NationalId == request.NationalId, cancellationToken);

            if (nationalIdExists)
                return Result.Failure<PatientCreateResponseDto>(PatientErrors.DuplicateNationalId);
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            var emailExists = await _userManager.FindByEmailAsync(request.Email);
            if (emailExists is not null)
                return Result.Failure<PatientCreateResponseDto>(UserErrors.DuplicatedEmail);
        }

        var user = request.Adapt<ApplicationUser>();
        var identityResult = await _userManager.CreateAsync(user);

        if (!identityResult.Succeeded)
        {
            var error = identityResult.Errors.FirstOrDefault()?.Description ?? "Failed to create user";
            _logger.LogWarning("Failed to create ApplicationUser. Error: {Error}", error);

            return Result.Failure<PatientCreateResponseDto>(
                new Error("User.CreateFailed", error, StatusCodes.Status400BadRequest));
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "patients");
        Directory.CreateDirectory(uploadsFolder);

        string? storedImageName = null;

        try
        {
            if (request.ProfileImageUrl is not null)
            {
                storedImageName = $"{Guid.NewGuid()}{Path.GetExtension(request.ProfileImageUrl.FileName)}";
                var filePath = Path.Combine(uploadsFolder, storedImageName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await request.ProfileImageUrl.CopyToAsync(stream, cancellationToken);
            }

            var patient = request.Adapt<Patient>();
            patient.PatientId = Guid.NewGuid();
            patient.UserId = user.Id;

            if (request.ProfileImageUrl is not null)
            {
                patient.User.ProfileImageUrl = new UploadedFile
                {
                    FileName = request.ProfileImageUrl.FileName,
                    StoredFileName = storedImageName,
                    FileExtension = Path.GetExtension(request.ProfileImageUrl.FileName),
                    ContentType = request.ProfileImageUrl.ContentType
                };
            }

            await _context.Patients.AddAsync(patient, cancellationToken);

            if (request.InitialBooking is not null)
            {
                var appointment = request.InitialBooking.Adapt<Appointment>();
                appointment.Id = Guid.NewGuid();
                appointment.PatientId = patient.PatientId;

                await _context.Appointments.AddAsync(appointment, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var response = patient.Adapt<PatientCreateResponseDto>();

            _logger.LogInformation("Patient created successfully with ID: {PatientId}", patient.PatientId);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating patient");

            if (!string.IsNullOrEmpty(storedImageName))
            {
                var filePath = Path.Combine(uploadsFolder, storedImageName);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            return Result.Failure<PatientCreateResponseDto>(
                new Error("Patient.CreateFailed", "Unexpected error occurred while creating patient"));
        }
    }

    public async Task<Result> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.PatientId == id, cancellationToken);

        if (patient is null)
            return Result.Failure(PatientErrors.NotFound);

        patient.IsDeleted = true;
        patient.DeletedOn = DateTime.UtcNow;

        _context.Patients.Update(patient);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<ResponseAllAppointmentPatient>>> GetAllAppointmentsByPatientIdAsync(Guid patientId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.PatientId == patientId);

        if (patient == null)
        {
            return Result.Failure<IEnumerable<ResponseAllAppointmentPatient>>(PatientErrors.NotFound);
        }

        var appointments = await _context.Appointments
            .Where(a => a.PatientId == patientId)
            .Include(a => a.Doctor)
            .ToListAsync();

        if (appointments == null || !appointments.Any())
        {
            return Result.Success(Enumerable.Empty<ResponseAllAppointmentPatient>());
        }

        var response = appointments.Adapt<IEnumerable<ResponseAllAppointmentPatient>>();

        return Result.Success(response);
    }

    public async Task<Result<PatientResponseDto>> GetPatientByIdAsync(Guid id)
    {
        var patient = await _context.Patients
            .Include(p => p.User)
                .ThenInclude(u => u.ProfileImageUrl)
            .FirstOrDefaultAsync(p => p.PatientId == id);

        if (patient is null)
        {
            _logger.LogWarning("Patient with ID {Id} not found.", id);
            return Result.Failure<PatientResponseDto>(PatientErrors.NotFound);
        }

        var response = patient.Adapt<PatientResponseDto>();
        return Result.Success(response);
    }

    public async Task<Result<PagedPatientResponse>> GetPatientsAsync(string? search, int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 10;

        IQueryable<Patient> query = _context.Patients.Include(p => p.User);

        if (!string.IsNullOrWhiteSpace(search))
        {
            string normalizedSearch = search.Trim().ToLower();
            query = query.Where(p =>
                p.User.FullName.ToLower().Contains(normalizedSearch) ||
                p.User.PhoneNumber.Contains(normalizedSearch) ||
                (p.NationalId != null && p.NationalId.Contains(normalizedSearch))
            );
        }

        int totalCount = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var patients = await query
            .OrderBy(p => p.User.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        var patientDtos = patients.Adapt<IEnumerable<PatientListResponseDto>>();

        var response = new PagedPatientResponse
        (
            Data: patientDtos,
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount,
            TotalPages: totalPages
        );

        return Result.Success(response);
    }

    public async Task<Result<PatientResponseDto?>> UpdatePatientAsync(Guid id, PatientRequestDto request)
    {
        var patient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.PatientId == id);

        if (patient is null)
        {
            return Result.Failure<PatientResponseDto?>(PatientErrors.NotFound);
        }

        if (patient.User.PhoneNumber != request.PhoneNumber)
        {
            var phoneExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber && u.Id != patient.UserId);
            if (phoneExists) return Result.Failure<PatientResponseDto?>(UserErrors.DuplicatePhoneNumber);
        }

        if (patient.User.Email != request.Email && !string.IsNullOrEmpty(request.Email))
        {
            var emailExists = await _userManager.FindByEmailAsync(request.Email);
            if (emailExists is not null && emailExists.Id != patient.UserId) return Result.Failure<PatientResponseDto?>(UserErrors.DuplicatedEmail);
        }

        if (patient.NationalId != request.NationalId && !string.IsNullOrEmpty(request.NationalId))
        {
            var nationalIdExists = await _context.Patients.AnyAsync(p => p.NationalId == request.NationalId && p.PatientId != id);
            if (nationalIdExists) return Result.Failure<PatientResponseDto?>(PatientErrors.DuplicateNationalId);
        }

        request.Adapt(patient.User);
        request.Adapt(patient);

        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();

        var response = patient.Adapt<PatientResponseDto>();
        return Result.Success(response);
    }

    public async Task<Result<IEnumerable<ResponsePatientInvoice>>> GetAllInvoicesByPatientIdAsync(Guid patientId)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
        if (patient is null)
        {
            return Result.Failure<IEnumerable<ResponsePatientInvoice>>(PatientErrors.NotFound);
        }

        var invoices = await _context.Invoices
            .Where(i => i.PatientId == patientId)
            .OrderByDescending(i => i.CreatedDate)
            .ToListAsync();

        var response = invoices.Adapt<IEnumerable<ResponsePatientInvoice>>();

        return Result.Success(response);
    }

    public async Task<Result<IEnumerable<ResponsePatientStay>>> GetAllStaysByPatientIdAsync(Guid patientId)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
        if (patient is null)
        {
            return Result.Failure<IEnumerable<ResponsePatientStay>>(PatientErrors.NotFound);
        }

        var stays = await _context.Stays
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.CheckInDate)
            .ToListAsync();

        var response = stays.Adapt<IEnumerable<ResponsePatientStay>>();

        return Result.Success(response);
    }

    public async Task<Result<IEnumerable<ResponsePatientOperation>>> GetAllOperationsByPatientIdAsync(Guid patientId)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
        if (patient is null)
        {
            return Result.Failure<IEnumerable<ResponsePatientOperation>>(PatientErrors.NotFound);
        }

        var operations = await _context.Operations
            .Where(o => o.PatientId == patientId)
            .OrderByDescending(o => o.Date)
            .ToListAsync();

        var response = operations.Adapt<IEnumerable<ResponsePatientOperation>>();

        return Result.Success(response);
    }
}