using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Stay.Requests;
using ClincManagement.API.Contracts.Stay.Respones;
using ClincManagement.API.Entities;
using ClincManagement.API.Errors;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ClincManagement.API.Services
{
    public class StayService : IStayService
    {
        private readonly ApplicationDbContext _context;

        public StayService(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<Result<StayDto>> CreateStayAsync(CreateStayDto request, CancellationToken cancellationToken)
        {
            var patientExists = await _context.Patients.AnyAsync(p => p.PatientId == request.PatientId, cancellationToken);
            if (!patientExists)
                return Result.Failure<StayDto>(StayErrors.InvalidPatient);

            var existingStay = await _context.Stays
                .AnyAsync(s => s.PatientId == request.PatientId && s.Status != "Discharged" && !s.IsDeleted, cancellationToken);

            if (existingStay)
                return Result.Failure<StayDto>(StayErrors.AlreadyExists);

            try
            {
                var stay = request.Adapt<Stay>();
                stay.Id = Guid.CreateVersion7();
                stay.Status = "Active";
                stay.CheckInDate = DateTime.UtcNow;
                stay.IsDeleted = false;
                stay.UpdatedOn = DateTime.UtcNow;


                if (request.ServiceIds is not null && request.ServiceIds.Any())
                {
                    var services = await _context.MedicalServices
                        .Where(s => request.ServiceIds.Contains(s.Id))
                        .ToListAsync(cancellationToken);

                    stay.MedicalServices = services;
                    stay.TotalCost = services.Sum(s => s.Cost);
                }


                _context.Stays.Add(stay);

                var activity = new StayActivity
                {
                    StayId = stay.Id,
                    Action = "Created",
                    ByUser = "System",
                    Description = $"Stay created for patient {stay.PatientId}",
                    CreatedOn = DateTime.UtcNow
                };
                _context.StayActivities.Add(activity);

                await _context.SaveChangesAsync(cancellationToken);

                var dto = stay.Adapt<StayDto>();
                dto.PatientName = await _context.Patients
                    .Where(p => p.PatientId == stay.PatientId)
                    .Select(p => p.User.UserName)
                    .FirstOrDefaultAsync(cancellationToken) ?? "Unknown";

                dto.ActivityLog = new List<ActivityLogDto>
                {
                    new ActivityLogDto
                    {
                        Id = activity.Id,
                        Action = activity.Action,
                        ByUser = activity.ByUser,
                        Timestamp = activity.CreatedOn
                    }
                };

                return Result.Success(dto);
            }
            catch
            {
                return Result.Failure<StayDto>(StayErrors.CreationFailed);
            }
        }

        public async Task<Result> UpdateStayAsync(Guid stayId, UpdateStayDto request, CancellationToken cancellationToken)
        {
            var stay = await _context.Stays
                .FirstOrDefaultAsync(s => s.Id == stayId && !s.IsDeleted, cancellationToken);

            if (stay is null)
                return Result.Failure(StayErrors.NotFound);

            try
            {
                stay.Department = request.Department;
                stay.RoomNumber = request.RoomNumber;
                stay.BedNumber = request.BedNumber;
                stay.Status = request.Status;
                stay.Notes = request.Notes;
                stay.CheckOutDate = request.CheckOutDate;
                stay.UpdatedOn = DateTime.UtcNow;

                if (request.ServiceIds is not null && request.ServiceIds.Any())
                {
                    var services = await _context.MedicalServices
                        .Where(s => request.ServiceIds.Contains(s.Id))
                        .ToListAsync(cancellationToken);

                    stay.MedicalServices = services;
                    stay.TotalCost = services.Sum(s => s.Cost);
                }


                _context.Stays.Update(stay);

             
                var activity = new StayActivity
                {
                    StayId = stay.Id,
                    Action = "Updated",
                    ByUser = "System",
                    Description = $"Stay updated ({stay.Status})",
                    CreatedOn = DateTime.UtcNow
                };
                _context.StayActivities.Add(activity);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch
            {
                return Result.Failure(StayErrors.UpdateFailed);
            }
        }

        public async Task<Result> DeleteStayAsync(Guid stayId, CancellationToken cancellationToken)
        {
            var stay = await _context.Stays.FirstOrDefaultAsync(s => s.Id == stayId, cancellationToken);
            if (stay is null)
                return Result.Failure(StayErrors.NotFound);

            try
            {
                stay.IsDeleted = true;
                stay.UpdatedOn = DateTime.UtcNow;

                _context.Stays.Update(stay);

                var activity = new StayActivity
                {
                    StayId = stay.Id,
                    Action = "Deleted",
                    ByUser = "System",
                    Description = "Stay record deleted.",
                    CreatedOn = DateTime.UtcNow
                };
                _context.StayActivities.Add(activity);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch
            {
                return Result.Failure(StayErrors.DeletionFailed);
            }
        }

 
        public async Task<Result<StayDto>> GetStayByIdAsync(Guid stayId, CancellationToken cancellationToken)
        {
            var stay = await _context.Stays
                .Include(s => s.Patient)
                .Include(s => s.ActivityLog)
                .FirstOrDefaultAsync(s => s.Id == stayId && !s.IsDeleted, cancellationToken);

            if (stay is null)
                return Result.Failure<StayDto>(StayErrors.NotFound);

            var dto = stay.Adapt<StayDto>();
            dto.PatientName = $"{stay.Patient.User.UserName}";
            dto.ActivityLog = stay.ActivityLog
                .Select(a => new ActivityLogDto
                {
                    Id = a.Id,
                    Action = a.Action,
                    ByUser = a.ByUser,
                    Timestamp = a.CreatedOn,
                }).ToList();

            return Result.Success(dto);
        }

       
        public async Task<Result<PagedStayResponse>> GetAllStaysAsync(
            string? department,
            string? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Stays
                .Include(s => s.Patient)
                .Include(s => s.ActivityLog)
                .Where(s => !s.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(department))
                query = query.Where(s => s.Department == department);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(s => s.Status == status);

            var totalCount = await query.CountAsync(cancellationToken);
            if (totalCount == 0)
                return Result.Failure<PagedStayResponse>(StayErrors.NotFound);

            var stays = await query
                .OrderByDescending(s => s.CheckInDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var data = stays.Select(stay => new StayDto
            {
                Id = stay.Id,
                PatientId = stay.PatientId,
                PatientName = stay.Patient.User.UserName,
                Department = stay.Department,
                Status = stay.Status,
                RoomNumber = stay.RoomNumber,
                BedNumber = stay.BedNumber,
                CheckInDate = stay.CheckInDate,
                CheckOutDate = stay.CheckOutDate,
                Notes = stay.Notes,
                ActivityLog = stay.ActivityLog.Select(a => new ActivityLogDto
                {
                    Id = a.Id,
                    Action = a.Action,
                    ByUser = a.ByUser,
                    Timestamp = a.CreatedOn,
                }).ToList()
            }).ToList();

            var response = new PagedStayResponse
            {
                Data = data,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Result.Success(response);
        }
    }
}
