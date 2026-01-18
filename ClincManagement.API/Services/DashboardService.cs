using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Dashboard;
using ClincManagement.API.Services.Interface;
using ClincManagement.API.Enums;
using ClincManagement.API.Entities;
using ClincManagement.API.Errors;
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

            var summary = new DashboardSummaryDto
            {
                TotalPatients = await _context.Patients
                    .AsNoTracking()
                    .CountAsync(),

                TodayAppointmentsCount = await _context.Appointments
                    .AsNoTracking()
                    .Where(a => EF.Functions.DateDiffDay(a.AppointmentDate, today) == 0)
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
                    .Where(p => EF.Functions.DateDiffDay(p.CreatedOn, today) == 0)
                    .CountAsync()
            };

            summary.TodayAppointments = await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                .Where(a => EF.Functions.DateDiffDay(a.AppointmentDate, today) == 0)
                .OrderBy(a => a.AppointmentTime)
                .Select(a => new AppointmentDto
                {
                    Time = a.AppointmentTime.ToString(@"hh\:mm"),
                    Patient = a.Patient.User.FullName,
                    Doctor = a.Doctor.FullName,
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
                    Patient = i.Patient.User.FullName,
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
