
using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Contracts.Patient.Responses;
using ClincManagement.API.Errors;
using ClincManagement.API.Services;
using ClincManagement.API.Services.Interface;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
            patient.PatientId = Guid.CreateVersion7();
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
            await _context.SaveChangesAsync(cancellationToken);

           
            if (request.InitialBooking is not null)
            {
                var appointment = request.InitialBooking.Adapt<Appointment>();
                appointment.Id = Guid.CreateVersion7();
                appointment.PatientId = patient.PatientId;

                await _context.Appointments.AddAsync(appointment, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

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

       
        patient.IsDeleted= true;
        patient.DeletedOn = DateTime.UtcNow;

        _context.Patients.Update(patient);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }


    public async Task<Result<IEnumerable<ResponseAllAppointmentPatient>>> GetAllAppointmentsByPatientIdAsync(Guid patientId)
    {
        var patient = await _context.Patients
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Doctor)
            .Include(p => p.Invoice)
            .FirstOrDefaultAsync(p => p.PatientId == patientId);

        if (patient == null)
        {
            return Result.Failure<IEnumerable<ResponseAllAppointmentPatient>>(PatientErrors.NotFound);
        }

        if (patient.Appointments == null || !patient.Appointments.Any())
        {
            return Result.Failure<IEnumerable<ResponseAllAppointmentPatient>>(PatientErrors.NoAppointments);
        }

     
        var appointments = patient.Appointments.Adapt<IEnumerable<ResponseAllAppointmentPatient>>();

        return Result.Success(appointments);
    }





}
