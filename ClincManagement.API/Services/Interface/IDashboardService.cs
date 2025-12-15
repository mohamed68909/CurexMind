using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Dashboard;

namespace ClincManagement.API.Services.Interface
{
  

        public interface IDashboardService
        {
            Task<Result<DashboardSummaryDto>> GetReceptionistSummaryAsync(Guid userId);
        }
    }

