using CurexMind.API.Abstractions;
using CurexMind.API.Contracts.Clinic.Respones;
using CurexMind.API.Contracts.Doctors.Requests;
using CurexMind.API.Contracts.Doctors.Respones;
using CurexMind.API.Contracts.Review.Requests;
using CurexMind.API.Contracts.Review.Respones;

namespace CurexMind.API.Services.Interface
{
    public interface IDoctorService
    {

        Task<Result<PagedDoctorResponse>> GetAll(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default);
        Task<Result<DoctorDetailsResponse>> GetAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result<DoctorDetailsResponse>> CreateAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default);


        Task<Result<DoctorDetailsResponse>> UpdateAsync(Guid id, UpdateDoctorRequest request, CancellationToken cancellationToken = default);


         Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);


        Task<Result<AddReviewResponse>> AddReview(Guid doctorId, string userId, AddReviewRequest request, CancellationToken cancellationToken = default);
    }
}
