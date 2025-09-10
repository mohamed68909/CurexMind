using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Contracts.Patient.Responses;
using ClincManagement.API.Services.Interface;
using System;

public class PatientService : IPatientService
{
    private readonly ApplicationDbContext _context;

    public PatientService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PatientResponseDto?> GetPatientByIdAsync(Guid id)
    {
        var patient = await _context.Patients
            .Include(x => x.User).Include(y => y.VitalSigns)
            .FirstOrDefaultAsync(p => p.PatientId == id);

        if (patient == null) return null;

        return new PatientResponseDto(
            patient.PatientId,
             patient.User.FullName,

            patient.Gender,
            CalculateAge(patient.DateOfBirth),
            patient.SocialStatus,
            patient.User.PhoneNumber,
            patient.User.Email,
            patient.NationalId,
            patient.User.Address,
            patient.Notes,
            patient.ProfileImageUrl,
        
            


            //new VitalSignsDto(
            //patient.VitalSigns.
                
            //),
            patient.CreatedDate
        );
    }

    public async Task<PagedPatientResponse> GetPatientsAsync(string? search, int page = 1, int pageSize = 10)
    {
        var query = _context.Patients.Include(n=>n.User).AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.User.FullName.Contains(search) || p.User.PhoneNumber.Contains(search));
        


        var totalCount = await query.CountAsync();

        var patients = await query
            .OrderByDescending(p => p.NationalId).Include(n=>n.User)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var data = patients.Select(p => new PatientListResponseDto(
            p.PatientId,
            p.User.FullName,
            p.Gender,
            CalculateAge(p.DateOfBirth),
 
            p.User.PhoneNumber,
            p.User.Address
          



        ));

        return new PagedPatientResponse(
            data,
            totalCount,
            page,
            pageSize,
            (int)Math.Ceiling((double)totalCount / pageSize)
        );
    }

    public async Task<PatientCreateResponseDto> CreatePatientAsync(PatientRequestDto request)
    {
       
        var patient = new Patient
        {
           PatientId= Guid.CreateVersion7(),

           Gender =request.Gender,
            User = new ApplicationUser
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
               
            },
            NationalId = request.NationalId,
            DateOfBirth = request.DateOfBirth,
           
            Notes = request.Notes ?? string.Empty,
            CreatedDate = DateTime.UtcNow,

            SocialStatus = request.SocialStatus.GetValueOrDefault(),

          
            
        };

       
        _context.Patients.Add(patient);

        await _context.SaveChangesAsync();

       
        var response = new PatientCreateResponseDto(patient.PatientId);

        return response;
    }


    public async Task<PatientResponseDto?> UpdatePatientAsync(Guid id, PatientRequestDto request)
    {
        var patient = await _context.Patients.Include(n=>n.User).FirstOrDefaultAsync(p => p.PatientId == id);
        if (patient == null) return null;

        patient.User.FullName= request.FullName;
        patient.Gender = request.Gender;

        patient.SocialStatus = request.SocialStatus.GetValueOrDefault();
        patient.User.PhoneNumber = request.PhoneNumber;
        patient.User.Email= request.Email;
        patient.NationalId = request.NationalId;
        patient.User.Address = request.Address;
        patient.Notes= request.Notes;
    
        await _context.SaveChangesAsync();

        return await GetPatientByIdAsync(patient.PatientId);
    }

    public async Task<bool> DeletePatientAsync(Guid id)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id);
        if (patient == null) return false;
        patient.IsActive = false; // Soft delete

        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
        return true;
    }

    private int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }

   
}
