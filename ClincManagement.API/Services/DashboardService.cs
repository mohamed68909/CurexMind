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
            var today = DateTime.Today.Date;

            var summary = new DashboardSummaryDto
            {
                // ... (مقاييس عليا) ...
                TotalPatients = await _context.Patients.CountAsync(),
                TodayAppointmentsCount = await _context.Appointments
                    .Where(a => a.AppointmentDate.Date == today)
                    .CountAsync(),
                UnpaidInvoicesCount = await _context.Invoices
                    .Where(i => i.Status == InvoiceStatus.Due || i.Status == InvoiceStatus.Partial)
                    .CountAsync(),
                UnpaidInvoicesAmountEGP = await _context.Invoices
                    .Where(i => i.Status == InvoiceStatus.Due || i.Status == InvoiceStatus.Partial)
                    .SumAsync(i => i.FinalAmountEGP - i.PaidAmountEGP),
                NewPatientsToday = await _context.Patients
                    .Where(p => p.CreatedOn.Date == today)
                    .CountAsync()
            };


            // **جداول التفاصيل**

            // التصحيح في هذا الجزء
            summary.TodayAppointments = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor)

                .Where(a => a.AppointmentDate.Date == today)
                .OrderBy(a => a.AppointmentTime) // 🚀 التصحيح: الترتيب يكون على خاصية TimeSpan القابلة للترجمة

                .Select(a => new AppointmentDto
                {
                    // التحويل إلى نص يتم هنا (بعد الترتيب)
                    Time = a.AppointmentTime.ToString(@"hh\:mm"),
                    Patient = a.Patient.User.FullName,
                    Doctor = a.Doctor.FullName,
                    Status = a.Status.ToString()
                })
                .ToListAsync();

            // الفواتير للمتابعة (لا يوجد بها خطأ ترجمة)
            summary.InvoicesToFollowUp = await _context.Invoices
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
            var specificError = new Error(
                code: DashboardErrors.FetchFailed.Code,
                message: $"{DashboardErrors.FetchFailed.Message} Details: {ex.Message}",
                statusCode: DashboardErrors.FetchFailed.StatusCode
            );

            return Result.Failure<DashboardSummaryDto>(specificError);
        }

    }
}

