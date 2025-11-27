using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Stay.Requests;
using ClincManagement.API.Contracts.Stay.Respones.ClincManagement.API.Contracts.Stay.Responses;
using ClincManagement.API.Contracts.Stay.Respones;
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
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // تحقق من وجود المريض
                var patientExists = await _context.Patients.AnyAsync(p => p.PatientId == request.PatientId, cancellationToken);
                if (!patientExists)
                    return Result.Failure<StayDetailsResponse>(StayErrors.InvalidPatient);

                // تحقق من تضارب StayType / Dates (اختياري)
                var overlappingStay = await _context.Stays
                    .AnyAsync(s => s.PatientId == request.PatientId &&
                                   s.StartDate < (request.EndDate ?? DateTime.MaxValue) &&
                                   (s.EndDate ?? DateTime.MaxValue) > request.StartDate,
                              cancellationToken);
                if (overlappingStay)
                    return Result.Failure<StayDetailsResponse>(StayErrors.AlreadyExists);

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

                // جلب التفاصيل للـ Response
                return await GetStayByIdAsync(stay.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error creating stay");
                return Result.Failure<StayDetailsResponse>(StayErrors.CreationFailed);
            }
        }

        public async Task<Result> DeleteStayAsync(Guid stayId, CancellationToken cancellationToken = default)
        {
            var stay = await _context.Stays.FirstOrDefaultAsync(s => s.Id == stayId, cancellationToken);
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

        public async Task<Result<PagedStayResponse>> GetAllStaysAsync(string? department, string? status, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<Stay> query = _context.Stays.Include(s => s.Patient).ThenInclude(p => p.User);

            if (!string.IsNullOrWhiteSpace(department))
                query = query.Where(s => s.Department.ToLower().Contains(department.ToLower()));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(s => s.Status.ToString() == status);

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

        public async Task<Result> UpdateStayAsync(Guid stayId, UpdateStayRequest request, CancellationToken cancellationToken = default)
        {
            var stay = await _context.Stays.FirstOrDefaultAsync(s => s.Id == stayId, cancellationToken);
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
    }
}
