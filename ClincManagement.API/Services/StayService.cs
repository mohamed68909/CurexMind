using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Stay.Requests;
using ClincManagement.API.Contracts.Stay.Responses;
using ClincManagement.API.Entities;
using ClincManagement.API.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClincManagement.API.Services
{
    public class StayService : IStayService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StayService> _logger;

        public StayService(ApplicationDbContext context, ILogger<StayService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<StayDetailsResponse>> CreateStayAsync(CreateStayRequest request, CancellationToken cancellationToken = default)
        {
            // 1. Validation Logic
            if (request.EndDate.HasValue && request.EndDate <= request.StartDate)
            {
                return Result.Failure<StayDetailsResponse>(new Error("Stay.InvalidDates", "End date must be after start date.", 400));
            }

            if (!Enum.IsDefined(typeof(StayType), request.StayType))
            {
                return Result.Failure<StayDetailsResponse>(new Error("Stay.InvalidType", "Invalid Stay Type value.", 400));
            }

            // 2. Use Execution Strategy to handle transactions safely with Retry Logic
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // A. Check Patient
                    var patient = await _context.Patients
                        .Include(p => p.User)
                        .FirstOrDefaultAsync(p => p.PatientId == request.PatientId, cancellationToken);

                    if (patient == null)
                        return Result.Failure<StayDetailsResponse>(StayErrors.InvalidPatient);

                    // B. Check Overlap
                    var overlappingStay = await _context.Stays
                        .AnyAsync(s => s.PatientId == request.PatientId &&
                                       s.Status == StayStatus.Active &&
                                       s.StartDate < (request.EndDate ?? DateTime.MaxValue) &&
                                       (s.EndDate ?? DateTime.MaxValue) > request.StartDate,
                                   cancellationToken);

                    if (overlappingStay)
                        return Result.Failure<StayDetailsResponse>(StayErrors.AlreadyExists);

                    // C. Create Stay
                    var stay = new Stay
                    {
                        Id = Guid.NewGuid(),
                        PatientId = request.PatientId,
                        Department = request.Department,
                        RoomNumber = request.RoomNumber,
                        BedNumber = request.BedNumber,
                        StayType = request.StayType,
                        Status = StayStatus.Active,
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        Notes = request.Notes
                    };

                    await _context.Stays.AddAsync(stay, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    // D. Build Response
                    var response = new StayDetailsResponse
                    {
                        StayId = stay.Id,
                        Patient = new PatientInfoDto
                        {
                            FullName = patient.User.FullName,
                            Gender = patient.Gender.ToString(),
                            Age = (int)((DateTime.Today - patient.DateOfBirth).TotalDays / 365.25),
                            PhoneNumber = patient.User.PhoneNumber ?? string.Empty
                        },
                        Stay = new StayInfoDto
                        {
                            Department = stay.Department,
                            RoomBed = $"{stay.RoomNumber}/{stay.BedNumber}",
                            StayType = stay.StayType.ToString(),
                            StartDate = stay.StartDate,
                            EndDate = stay.EndDate,
                            Status = stay.Status.ToString(),
                            Notes = stay.Notes
                        }
                    };

                    return Result.Success(response);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error creating stay");

                    var errorMessage = ex.InnerException?.Message ?? ex.Message;
                    return Result.Failure<StayDetailsResponse>(new Error("Stay.Exception", errorMessage, 500));
                }
            });
        }
        public async Task<Result> UpdateStayAsync(Guid stayId, UpdateStayRequest request, CancellationToken cancellationToken = default)
        {
            var stay = await _context.Stays.FindAsync(new object[] { stayId }, cancellationToken);
            if (stay == null) return Result.Failure(StayErrors.NotFound);

            stay.Department = request.Department;
            stay.RoomNumber = request.RoomNumber;
            stay.BedNumber = request.BedNumber;
            stay.StayType = request.StayType;
            stay.Status = request.Status;
            stay.StartDate = request.StartDate;
            stay.EndDate = request.EndDate;
            stay.Notes = request.Notes;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stay");
                return Result.Failure(StayErrors.UpdateFailed);
            }
        }

        public async Task<Result> DeleteStayAsync(Guid stayId, CancellationToken cancellationToken = default)
        {
            var stay = await _context.Stays.FindAsync(new object[] { stayId }, cancellationToken);
            if (stay == null) return Result.Failure(StayErrors.NotFound);

            _context.Stays.Remove(stay);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting stay");
                return Result.Failure(StayErrors.DeletionFailed);
            }
        }

        public async Task<Result<StayDetailsResponse>> GetStayByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var stay = await _context.Stays
                .Include(s => s.Patient)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (stay == null) return Result.Failure<StayDetailsResponse>(StayErrors.NotFound);

            var response = new StayDetailsResponse
            {
                StayId = stay.Id,
                Patient = new PatientInfoDto
                {
                    FullName = stay.Patient.User.FullName,
                    Gender = stay.Patient.Gender.ToString(),
                    Age = (int)((DateTime.Today - stay.Patient.DateOfBirth).TotalDays / 365.25),
                    PhoneNumber = stay.Patient.User.PhoneNumber ?? string.Empty
                },
                Stay = new StayInfoDto
                {
                    Department = stay.Department,
                    RoomBed = $"{stay.RoomNumber}/{stay.BedNumber}",
                    StayType = stay.StayType.ToString(),
                    StartDate = stay.StartDate,
                    EndDate = stay.EndDate,
                    Status = stay.Status.ToString(),
                    Notes = stay.Notes
                }
            };

            return Result.Success(response);
        }

        public async Task<Result<PagedStayResponse>> GetAllStaysAsync(string? department, string? status, int page, int pageSize, CancellationToken cancellationToken = default)
        {
        
            pageSize = Math.Min(pageSize, 100);

            IQueryable<Stay> query = _context.Stays.Include(s => s.Patient).ThenInclude(p => p.User);

            if (!string.IsNullOrWhiteSpace(department))
                query = query.Where(s => s.Department.ToLower().Contains(department.ToLower()));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(s => s.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));

            var total = await query.CountAsync(cancellationToken);

            var data = await query
                .OrderByDescending(s => s.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var list = data.Select(s => new StayListResponse
            {
                StayId = s.Id,
                PatientName = s.Patient.User.FullName,
                Department = s.Department,
                RoomBed = $"{s.RoomNumber}/{s.BedNumber}",
                StayType = s.StayType.ToString(),
                Date = s.StartDate,
                Status = s.Status.ToString()
            }).ToList();

            return Result.Success(new PagedStayResponse
            {
                TotalCount = total,
                Stays = list
            });
        }
    }
}
