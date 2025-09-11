using ClincManagement.API.Contracts.Clinic.Respones;
using ClincManagement.API.Contracts.Doctors.Respones;
using ClincManagement.API.Contracts.Review.Requests;
using ClincManagement.API.Contracts.Review.Respones;
using ClincManagement.API.Entities;
using ClincManagement.API.Errors;
using ClincManagement.API.Services.Interface;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ClincManagement.API.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;

        public DoctorService(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<Result<IEnumerable<DoctorListResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            var doctors = await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Clinic)
                .ToListAsync(cancellationToken);

            if (!doctors.Any())
                return Result.Failure<IEnumerable<DoctorListResponse>>(DoctorErrors.NotFound);

            var response = doctors.Adapt<IEnumerable<DoctorListResponse>>();
            return Result.Success(response);
        }


        public async Task<Result<DoctorDetailsResponse>> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var doctor = await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Clinic)
                .Include(d => d.Appointments)
                .Include(d => d.Reviews)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (doctor is null)
                return Result.Failure<DoctorDetailsResponse>(DoctorErrors.NotFound);

            var response = doctor.Adapt<DoctorDetailsResponse>();
            return Result.Success(response);
        }

    
        public async Task<Result<AddReviewResponse>> AddReview(Guid doctorId, string userId, AddReviewRequest request, CancellationToken cancellationToken = default)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Reviews)
                .FirstOrDefaultAsync(d => d.Id == doctorId, cancellationToken);

            if (doctor is null)
                return Result.Failure<AddReviewResponse>(DoctorErrors.NotFound);

   
            var review = new Review
            {
                Id = Guid.NewGuid(),
                DoctorId = doctorId,
                PatientId = Guid.Parse(userId),
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reviews.AddAsync(review, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var response = review.Adapt<AddReviewResponse>();
            return Result.Success(response);
        }
    }
}
