using ClinicManagement.API.Abstractions;
using ClinicManagement.API.Contracts.Stay.Requests;
using ClinicManagement.API.Contracts.Stay.Responses; 
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicManagement.API.Abstractions
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
