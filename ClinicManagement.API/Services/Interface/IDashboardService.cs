using ClinicManagement.API.Abstractions;
using ClinicManagement.API.Contracts.Dashboard;

namespace ClinicManagement.API.Services.Interface
{
  

        public interface IDashboardService
        {
            Task<Result<DashboardSummaryDto>> GetReceptionistSummaryAsync(Guid userId);
        }
    }

