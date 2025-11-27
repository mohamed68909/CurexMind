using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Stay.Requests;
using ClincManagement.API.Contracts.Stay.Respones.ClincManagement.API.Contracts.Stay.Responses;
using ClincManagement.API.Contracts.Stay.Respones;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClincManagement.API.Abstractions
{
    public interface IStayService
    {
        Task<Result<PagedStayResponse>> GetAllStaysAsync(
            string? department,
            string? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result<StayDetailsResponse>> GetStayByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<StayDetailsResponse>> CreateStayAsync(
            CreateStayRequest request,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateStayAsync(
            Guid stayId,
            UpdateStayRequest request,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteStayAsync(
            Guid stayId,
            CancellationToken cancellationToken = default);
    }
}
