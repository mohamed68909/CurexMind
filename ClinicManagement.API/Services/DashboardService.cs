using ClinicManagement.API.Abstractions;
using ClinicManagement.API.Contracts.Dashboard;
using ClinicManagement.API.Services.Interface;
using ClinicManagement.API.Enums;
using ClinicManagement.API.Entities;
using ClinicManagement.API.Errors;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DashboardSummaryDto>> GetReceptionistSummaryAsync(Guid userId)
    {
        try
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var todayUtc = DateTime.UtcNow.Date;
            var tomorrowUtc = todayUtc.AddDays(1);

            var summary = new DashboardSummaryDto
            {
                TotalPatients = await _context.Patients
                    .AsNoTracking()
                    .CountAsync(),

                TodayAppointmentsCount = await _context.Appointments
                    .AsNoTracking()
                    .Where(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow)
                    .CountAsync(),

                UnpaidInvoicesCount = await _context.Invoices
                    .AsNoTracking()
                    .Where(i => i.Status == InvoiceStatus.Due || i.Status == InvoiceStatus.Partial)
                    .CountAsync(),

                UnpaidInvoicesAmountEGP = await _context.Invoices
                    .AsNoTracking()
                    .Where(i => i.Status == InvoiceStatus.Due || i.Status == InvoiceStatus.Partial)
                    .SumAsync(i => (decimal?)(i.FinalAmountEGP - i.PaidAmountEGP)) ?? 0,

                NewPatientsToday = await _context.Patients
                    .AsNoTracking()
                    .Where(p => p.CreatedOn >= todayUtc && p.CreatedOn < tomorrowUtc)
                    .CountAsync()
            };

            summary.TodayAppointments = await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                .Where(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow)
                .OrderBy(a => a.AppointmentTime)
                .Select(a => new AppointmentDto
                {
                    Time = a.AppointmentTime.ToString(@"hh\:mm"),
                    Patient = a.Patient != null && a.Patient.User != null ? a.Patient.User.FullName : string.Empty,
                    Doctor = a.Doctor != null ? a.Doctor.FullName : string.Empty,
                    Status = a.Status.ToString()
                })
                .ToListAsync();

            summary.InvoicesToFollowUp = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.Patient).ThenInclude(p => p.User)
                .Where(i => i.Status == InvoiceStatus.Due || i.Status == InvoiceStatus.Partial)
                .OrderBy(i => i.DueDate)
                .Select(i => new InvoiceFollowUpDto
                {
                    Patient = i.Patient != null && i.Patient.User != null ? i.Patient.User.FullName : string.Empty,
                    AmountEGP = i.FinalAmountEGP - i.PaidAmountEGP,
                    Status = i.Status.ToString()
                })
                .ToListAsync();

            return Result<DashboardSummaryDto>.Success(summary);
        }
        catch (Exception ex)
        {
            return Result.Failure<DashboardSummaryDto>(
                new Error(
                    DashboardErrors.FetchFailed.Code,
                    $"{DashboardErrors.FetchFailed.Message} Details: {ex.Message}",
                    DashboardErrors.FetchFailed.StatusCode
                ));
        }
    }
}
