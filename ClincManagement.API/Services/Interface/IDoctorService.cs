using ClincManagement.API.Contracts.Clinic.Respones;
using ClincManagement.API.Contracts.Doctors.Respones;
using ClincManagement.API.Contracts.Review.Requests;
using ClincManagement.API.Contracts.Review.Respones;

namespace ClincManagement.API.Services.Interface
{
    public interface IDoctorService
    {
       Task<Result<IEnumerable<DoctorListResponse>>> GetAll(CancellationToken cancellationToken = default);
        Task<Result<DoctorDetailsResponse>> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<AddReviewResponse>> AddReview(Guid DoctorId, string UserId, AddReviewRequest request, CancellationToken cancellationToken = default);



    }
}
