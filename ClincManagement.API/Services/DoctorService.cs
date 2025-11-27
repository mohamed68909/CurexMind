using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Clinic.Respones;
using ClincManagement.API.Contracts.Doctors.Requests;
using ClincManagement.API.Contracts.Doctors.Respones;
using ClincManagement.API.Contracts.Review.Requests;
using ClincManagement.API.Contracts.Review.Respones;
using ClincManagement.API.Errors;
using ClincManagement.API.Services.Interface;
//is sccusee hent doctor add price 
public class DoctorService : IDoctorService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DoctorService> _logger;
    private readonly IImageFileService _imageFileService;

    public DoctorService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<DoctorService> logger,
        IImageFileService imageFileService)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
        _imageFileService = imageFileService;
    }

    
    public async Task<Result<DoctorDetailsResponse>> CreateAsync(
        CreateDoctorRequest request,
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Validate Clinic
            var clinic = await _context.Clinics.FindAsync(request.ClinicId);
            if (clinic == null)
                return Result.Failure<DoctorDetailsResponse>(DoctorErrors.ClinicNotFound);

            // Validate Duplicate Phone
            if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber))
                return Result.Failure<DoctorDetailsResponse>(UserErrors.DuplicatePhoneNumber);

            // Validate Duplicate Email
            if (!string.IsNullOrEmpty(request.Email) &&
                await _userManager.FindByEmailAsync(request.Email) != null)
                return Result.Failure<DoctorDetailsResponse>(UserErrors.DuplicatedEmail);

            // Create User
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
                var msg = identityResult.Errors.FirstOrDefault()?.Description;
                return Result.Failure<DoctorDetailsResponse>(new Error("Doctor.UserCreateFailed", msg!));
            }

            // Upload Image
            if (request.ProfileImage != null)
            {
                var uploaded = await _imageFileService.UploadAsync(request.ProfileImage, "uploads/doctors");
                user.ProfileImage = uploaded;
            }

            // Create doctor entity
            var doctor = new Doctor
            {
                Id = Guid.NewGuid(),
                ClinicId = request.ClinicId,
                UserId = user.Id,
               
                Specialization = request.Specialization,
                YearsOfExperience = request.YearsOfExperience,
                Languages = request.Languages,
                CreatedById = user.Id.ToString(),
                
            };

            await _context.Doctors.AddAsync(doctor, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success(new DoctorDetailsResponse
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Specialization = doctor.Specialization,
                ClinicName = clinic.Name,
                YearsOfExperience = doctor.YearsOfExperience,
                Languages = doctor.Languages,
                ProfileImageUrl = user.ProfileImage.StoredFileName
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);

            if (await _userManager.FindByNameAsync(request.UserName) is ApplicationUser u)
                await _userManager.DeleteAsync(u);
         
 

            _logger.LogError(ex, "Error creating doctor");

            return Result.Failure<DoctorDetailsResponse>(
                new Error("Doctor.CreateFailed", ex.Message));
        }
    }


    
    public async Task<Result<DoctorDetailsResponse>> GetAsync(Guid id, CancellationToken ct = default)
    {
        var doctor = await _context.Doctors
            .Include(d => d.Clinic)
            .Include(d => d.User).ThenInclude(u => u.ProfileImage)
            .Include(d => d.Reviews)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        if (doctor == null)
            return Result.Failure<DoctorDetailsResponse>(DoctorErrors.NotFound);

        return Result.Success(new DoctorDetailsResponse
        {
            Id = doctor.Id,
            FullName = doctor.FullName,
            Specialization = doctor.Specialization,
            ClinicName = doctor.Clinic.Name,
            YearsOfExperience = doctor.YearsOfExperience,
            Languages = doctor.Languages,
            ProfileImageUrl = doctor.User.ProfileImage.StoredFileName,
            Reviews = doctor.Reviews.Select(r => new ReviewResponse
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList()
        });
    }


  
    public async Task<Result<IEnumerable<DoctorListResponse>>> GetAll(CancellationToken ct = default)
    {
        var list = await _context.Doctors
            .Include(d => d.Clinic)
            .Include(d => d.User).ThenInclude(u => u.ProfileImage).Include(u=>u.Reviews)
            .ToListAsync(ct);

        return Result.Success(
            list.Select(d => new DoctorListResponse
            {
                Id = d.Id,
                FullName = d.FullName,
                Specialization = d.Specialization,
                ClinicName = d.Clinic.Name,
                ProfileImageUrl = d.User.ProfileImage.StoredFileName,
               ReviewsCount = d.Reviews.Count,
                Rating = d.Reviews.Count == 0 ? 0 : d.Reviews.Average(r => r.Rating)
               

            })
        );
    }


   
    public async Task<Result<DoctorDetailsResponse>> UpdateAsync(
        Guid id,
        UpdateDoctorRequest request,
        CancellationToken ct = default)
    {
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .ThenInclude(u => u.ProfileImage)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        if (doctor == null)
            return Result.Failure<DoctorDetailsResponse>(DoctorErrors.NotFound);

        var user = doctor.User;

        // Validate Email/Phone
        if (user.PhoneNumber != request.PhoneNumber &&
            await _userManager.Users.AnyAsync(x => x.PhoneNumber == request.PhoneNumber))
            return Result.Failure<DoctorDetailsResponse>(UserErrors.DuplicatePhoneNumber);

        if (user.Email != request.Email &&
            await _userManager.FindByEmailAsync(request.Email) != null)
            return Result.Failure<DoctorDetailsResponse>(UserErrors.DuplicatedEmail);

        // Update fields
        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.Email = request.Email;

        doctor.FullName = request.FullName;
        doctor.Specialization = request.Specialization;
        doctor.YearsOfExperience = request.YearsOfExperience;
        doctor.Languages = request.Languages;

        // Update image
        if (request.NewProfileImage != null)
        {
            if (user.ProfileImage != null)
                _imageFileService.Delete(user.ProfileImage, "uploads/doctors");

            var uploaded = await _imageFileService.UploadAsync(request.NewProfileImage, "uploads/doctors");
            user.ProfileImage = uploaded;
        }

        await _context.SaveChangesAsync(ct);

        return await GetAsync(id, ct);
    }


   
    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var doc = await _context.Doctors
            .Include(d => d.User).ThenInclude(u => u.ProfileImage)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        if (doc == null)
            return Result.Failure<bool>(DoctorErrors.NotFound);

        if (doc.User?.ProfileImage != null)
            _imageFileService.Delete(doc.User.ProfileImage, "uploads/doctors");

        doc.IsDeleted = true;
        doc.DeletedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return Result.Success(true);
    }


 
    public async Task<Result<AddReviewResponse>> AddReview(
        Guid doctorId, string userId, AddReviewRequest req, CancellationToken ct = default)
    {
        var doctor = await _context.Doctors
            .Include(d => d.Reviews)
            .FirstOrDefaultAsync(d => d.Id == doctorId, ct);

        if (doctor == null)
            return Result.Failure<AddReviewResponse>(DoctorErrors.NotFound);

        var review = new Review
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            PatientId = Guid.Parse( userId),
            Rating = req.Rating,
            Comment = req.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Reviews.AddAsync(review, ct);
        await _context.SaveChangesAsync(ct);

        return Result.Success(new AddReviewResponse { ReviewId = review.Id });
    }
}
