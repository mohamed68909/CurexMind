using CurexMind.API.Abstractions;
using CurexMind.API.Contracts.Dashboard;

namespace CurexMind.API.Services.Interface
{
  

        public interface IDashboardService
        {
            Task<Result<DashboardSummaryDto>> GetReceptionistSummaryAsync(Guid userId);
        }
    }

