using ClinicManagement.API.Abstractions;
using ClinicManagement.API.Contracts.Clinic.Respones;
using ClinicManagement.API.Contracts.Doctors.Requests;
using ClinicManagement.API.Contracts.Doctors.Respones;
using ClinicManagement.API.Contracts.Review.Requests;
using ClinicManagement.API.Contracts.Review.Respones;
using ClinicManagement.API.Errors;
using ClinicManagement.API.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.API.Services;

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
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                
                var clinic = await _context.Clinics.FindAsync(request.ClinicId);
                if (clinic == null)
                    return Result.Failure<DoctorDetailsResponse>(DoctorErrors.ClinicNotFound);

               
                if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber))
                    return Result.Failure<DoctorDetailsResponse>(UserErrors.DuplicatePhoneNumber);

               
                if (!string.IsNullOrEmpty(request.Email) &&
                    await _userManager.FindByEmailAsync(request.Email) != null)
                    return Result.Failure<DoctorDetailsResponse>(UserErrors.DuplicatedEmail);

                var user = new ApplicationUser
                {
                   
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

               
                if (request.ProfileImage != null)
                {
                    var uploaded = await _imageFileService.UploadAsync(request.ProfileImage, "uploads/doctors");
                    user.ProfileImage = uploaded;
                }

               
                var doctor = new Doctor
                {
                    Id = Guid.NewGuid(),
                    ClinicId = request.ClinicId,
                    FullName = request.FullName,
                    UserId = user.Id,
                    Specialization = request.Specialization,
                    YearsOfExperience = request.YearsOfExperience,
                    Languages = request.Languages,
                    Bio = request.Bio,
                    Price = request.Price
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
                    ProfileImageUrl = user.ProfileImage?.StoredFileName,
                    Bio = doctor.Bio,
                    Price = doctor.Price
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                _logger.LogError(ex, "Error creating doctor");

                return Result.Failure<DoctorDetailsResponse>(
                    new Error("Doctor.CreateFailed", ex.Message));
            }
        });
    }

    public async Task<Result<DoctorDetailsResponse>> GetAsync(Guid id, CancellationToken ct = default)
    {
       var doctor = await _context.Doctors
    .Include(d => d.Clinic)
    .Include(d => d.User)
        .ThenInclude(u => u.ProfileImage)
    .Include(d => d.Reviews)
        
            .ThenInclude(p => p.User)
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
            Bio = doctor.Bio,
            Price = doctor.Price,

            ProfileImageUrl = doctor.User?.ProfileImage?.StoredFileName,
            Reviews = doctor.Reviews.Select(r => new ReviewResponse
            {
                Id = r.Id,
                PatientName = r.User?.FullName ?? "Unknown",
               
                Rating =  r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList()
        });
    }

    public async Task<Result<PagedDoctorResponse>> GetAll(int page = 1, int pageSize = 10, string? search = null, CancellationToken ct = default)
    {
        IQueryable<Doctor> query = _context.Doctors
            .Include(d => d.Clinic)
            .Include(d => d.User).ThenInclude(u => u.ProfileImage)
            .Include(u => u.Reviews);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(d => 
                d.FullName.ToLower().Contains(searchLower) ||
                d.Specialization.ToLower().Contains(searchLower) ||
                d.Clinic.Name.ToLower().Contains(searchLower));
        }

        var totalCount = await query.CountAsync(ct);

        var list = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var data = list.Select(d => new DoctorListResponse
        {
            Id = d.Id,
            FullName = d.FullName,
            Specialization = d.Specialization,
            ClinicName = d.Clinic.Name,
            ProfileImageUrl = d.User?.ProfileImage?.StoredFileName ?? string.Empty,
            Price = d.Price,
            ReviewsCount = d.Reviews.Count,
            Rating = d.Reviews.Count == 0 ? 0 : d.Reviews.Average(r => r.Rating)
        }).ToList();

        var response = new PagedDoctorResponse(
            Data: data,
            TotalCount: totalCount,
            Page: page,
            PageSize: pageSize,
            TotalPages: (int)Math.Ceiling(totalCount / (double)pageSize)
        );

        return Result.Success(response);
    }

    public async Task<Result<DoctorDetailsResponse>> UpdateAsync(
        Guid id,
        UpdateDoctorRequest request,
        CancellationToken ct = default)
    {
        var doctor = await _context.Doctors
            .Include(d => d.User)
            .ThenInclude(u => u.ProfileImage)
            .Include(d => d.Clinic)
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
        doctor.Bio = request.Bio;
        doctor.Price = request.Price;

        // Update image
        if (request.NewProfileImage != null)
        {
            if (user.ProfileImage != null)
                _imageFileService.Delete(user.ProfileImage, "uploads/doctors");

            var uploaded = await _imageFileService.UploadAsync(request.NewProfileImage, "uploads/doctors");
            user.ProfileImage = uploaded;
        }

        await _context.SaveChangesAsync(ct);

      
        return Result.Success(new DoctorDetailsResponse
        {
            Id = doctor.Id,
            FullName = doctor.FullName,
            Specialization = doctor.Specialization,
            ClinicName = doctor.Clinic?.Name,
            YearsOfExperience = doctor.YearsOfExperience,
            Languages = doctor.Languages,
            Price = doctor.Price,
            ProfileImageUrl = user.ProfileImage?.StoredFileName
        });
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
     Guid doctorId,
     string userId,
     AddReviewRequest req,
     CancellationToken ct = default)
    {
        
        var doctorExists = await _context.Doctors
            .AnyAsync(d => d.Id == doctorId, ct);

        if (!doctorExists)
            return Result.Failure<AddReviewResponse>(DoctorErrors.NotFound);

      
      //  var userExists = await _context.Users
           // .AnyAsync(u => u.Id == userId, ct);

       // if (!userExists)
          //  return Result.Failure<AddReviewResponse>(UserErrors.NotFound);

       
        var alreadyReviewed = await _context.Reviews.AnyAsync(
            r => r.DoctorId == doctorId && r.UserId == userId, ct);

        if (alreadyReviewed)
            return Result.Failure<AddReviewResponse>(ReviewErrors.AlreadyReviewed);

   
        var review = new Review
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            UserId = userId,
            Rating = req.Rating,
            Comment = req.Comment,
            CreatedAt = DateTime.UtcNow
        };

      
        await _context.Reviews.AddAsync(review, ct);
        await _context.SaveChangesAsync(ct);

        return Result.Success(new AddReviewResponse
        {
            ReviewId = review.Id
        });
    }


}