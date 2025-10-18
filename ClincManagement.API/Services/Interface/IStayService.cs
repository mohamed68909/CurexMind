using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Stay.Requests;
using ClincManagement.API.Contracts.Stay.Respones;


public interface IStayService
{
    Task<Result<PagedStayResponse>> GetAllStaysAsync(
        string? department,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancel);


    Task<Result<StayDto>> GetStayByIdAsync(Guid id, CancellationToken cancel);

    Task<Result<StayDto>> CreateStayAsync(CreateStayDto request, CancellationToken cancel);


    Task<Result> UpdateStayAsync(Guid stayId, UpdateStayDto request, CancellationToken cancel);


    Task<Result> DeleteStayAsync(Guid stayId, CancellationToken cancel);
}