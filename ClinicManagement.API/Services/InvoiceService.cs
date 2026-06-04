using ClinicManagement.API.Abstractions;
using ClinicManagement.API.Contracts.Invoice.Requests;
using ClinicManagement.API.Contracts.Invoice.Responses;
using ClinicManagement.API.Services.Interfaces;
using ClinicManagement.API.Errors;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClinicManagement.API.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE INVOICE
        public async Task<Result<InvoiceDetailsDto>> CreateInvoiceAsync(CreateInvoiceDto request, CancellationToken cancel = default)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == request.PatientId, cancel);

            if (patient == null)
                return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.InvalidPatient);

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Clinic)
                .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancel);

            if (doctor == null)
                return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.InvalidDoctor);

            if (!Enum.TryParse<InvoiceStatus>(request.PaymentInformation.PaymentStatus, out var status))
                return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.GeneralFailure);

            if (request.AmountDetails.TotalAmountEGP < 0 || request.AmountDetails.FinalAmountEGP < 0)
                return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.InvalidAmount);

            var invoice = new Invoice
            {
                Id = Guid.CreateVersion7(),
                InvoiceDate = DateTime.UtcNow,
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
                PatientId = patient.PatientId,
                DoctorId = doctor.Id,
                ClinicId = doctor.ClinicId,
                ServiceTypeId = request.ServiceDetails.ServiceTypeId,
                VisitDate = request.ServiceDetails.VisitDate,
                TotalAmountEGP = request.AmountDetails.TotalAmountEGP,
                DiscountEGP = request.AmountDetails.DiscountEGP,
                FinalAmountEGP = request.AmountDetails.FinalAmountEGP,
                PaymentMethod = request.PaymentInformation.PaymentMethod,
                Status = status,
                PaidAmountEGP = request.PaymentInformation.AmountPaidEGP,
                Notes = request.Notes,
                IsDeleted = false
            };

            await _context.Invoices.AddAsync(invoice, cancel);
            await _context.SaveChangesAsync(cancel);

            return Result.Success(ConvertToDetailsDto(invoice, patient, doctor));
        }

        // UPDATE INVOICE
        public async Task<Result<InvoiceDetailsDto>> UpdateInvoiceAsync(Guid invoiceId, UpdateInvoiceDto request, CancellationToken cancel = default)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Patient).ThenInclude(p => p.User)
                .Include(i => i.Doctor).ThenInclude(d => d.Clinic)
                .Include(i => i.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted, cancel);

            if (invoice == null)
                return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.NotFound);

            if (invoice.Status == InvoiceStatus.Paid)
                return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.AlreadyPaid);

            if (request.PatientId != Guid.Empty)
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == request.PatientId, cancel);
                if (patient == null)
                    return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.InvalidPatient);
                invoice.PatientId = request.PatientId;
            }

            if (request.DoctorId != Guid.Empty)
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancel);
                if (doctor == null)
                    return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.InvalidDoctor);
                invoice.DoctorId = request.DoctorId;
            }

            if (request.AmountDetails != null)
            {
                if (request.AmountDetails.TotalAmountEGP < 0 || request.AmountDetails.FinalAmountEGP < 0)
                    return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.InvalidAmount);

                invoice.TotalAmountEGP = request.AmountDetails.TotalAmountEGP;
                invoice.DiscountEGP = request.AmountDetails.DiscountEGP;
                invoice.FinalAmountEGP = request.AmountDetails.FinalAmountEGP;
            }

            if (request.PaymentInformation != null)
            {
                if (!Enum.TryParse<InvoiceStatus>(request.PaymentInformation.PaymentStatus, out var newStatus))
                    return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.GeneralFailure);

                invoice.PaymentMethod = request.PaymentInformation.PaymentMethod;
                invoice.Status = newStatus;
                invoice.PaidAmountEGP = request.PaymentInformation.AmountPaidEGP;
            }

            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync(cancel);

            return Result.Success(ConvertToDetailsDto(invoice, invoice.Patient, invoice.Doctor));
        }

        // DELETE INVOICE
        public async Task<Result> DeleteInvoiceAsync(Guid invoiceId, CancellationToken cancel = default)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted, cancel);

            if (invoice == null)
                return Result.Failure(InvoiceErrors.NotFound);

            invoice.IsDeleted = true;

            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync(cancel);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<InvoiceSummaryDto>>> GetAllInvoicesAsync(CancellationToken cancel = default)
        {
            var invoices = await _context.Invoices
                .Where(i => !i.IsDeleted)
                .Include(i => i.Patient).ThenInclude(p => p.User)
                .Include(i => i.Doctor).ThenInclude(d => d.User)
                .Include(i => i.ServiceType)
                .ToListAsync(cancel);

            if (!invoices.Any())
                return Result.Failure<IEnumerable<InvoiceSummaryDto>>(InvoiceErrors.NotFound);

            var list = invoices.Select(i => new InvoiceSummaryDto
            {
                InvoiceId = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate = i.InvoiceDate,

                PatientName = i.Patient?.User?.FullName ?? string.Empty,
                DoctorName = i.Doctor?.User?.FullName ?? string.Empty,

                ServiceType = i.ServiceType.Name,

                TotalAmount = i.TotalAmountEGP,
                DiscountApplied = i.DiscountEGP,
                NetTotal = i.FinalAmountEGP,

                PaymentMethod = i.PaymentMethod,
                PaymentStatus = i.Status.ToString()
            });

            return Result.Success(list);
        }



        // GENERATE PDF
        public async Task<Result<byte[]>> GeneratePdfExportAsync(Guid invoiceId, Guid? patientId = null, CancellationToken cancel = default)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Patient).ThenInclude(p => p.User)
                .Include(i => i.Doctor).ThenInclude(d => d.Clinic)
                .Include(i => i.Doctor).ThenInclude(d => d.User)
                .Include(i => i.ServiceType)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted, cancel);

            if (invoice == null)
                return Result.Failure<byte[]>(InvoiceErrors.NotFound);

            if (patientId.HasValue && invoice.PatientId != patientId.Value)
                return Result.Failure<byte[]>(UserErrors.AccessDenied);

            QuestPDF.Settings.License = LicenseType.Community;

            var patientName = invoice.Patient?.User?.FullName ?? "Unknown";
            var doctorName = invoice.Doctor?.User?.FullName ?? "Unknown";
            var clinicName = invoice.Doctor?.Clinic?.Name ?? "Unknown";
            var serviceTypeName = invoice.ServiceType?.Name ?? "General Consultation";

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("INVOICE")
                        .FontColor(Colors.Blue.Darken2).FontSize(24).Bold();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"Invoice Number: {invoice.InvoiceNumber}").Bold();
                                    col.Item().Text($"Invoice Date: {invoice.InvoiceDate:yyyy-MM-dd}");
                                    col.Item().Text($"Due Date: {invoice.DueDate?.ToString("yyyy-MM-dd") ?? "N/A"}");
                                });

                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"Patient: {patientName}").Bold();
                                    col.Item().Text($"Doctor: {doctorName}");
                                    col.Item().Text($"Clinic: {clinicName}");
                                });
                            });

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Description").Bold();
                                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Amount (EGP)").Bold();
                                });

                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"Medical Services - {serviceTypeName}");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{invoice.TotalAmountEGP:N2}");

                                if (invoice.DiscountEGP > 0)
                                {
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Discount");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"-{invoice.DiscountEGP:N2}");
                                }

                                table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Total").Bold();
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text($"{invoice.FinalAmountEGP:N2}").Bold();
                            });

                            column.Item().Text($"Status: {invoice.Status}").Bold();
                            column.Item().Text($"Payment Method: {invoice.PaymentMethod}");
                            if (!string.IsNullOrWhiteSpace(invoice.Notes))
                            {
                                column.Item().Text($"Notes: {invoice.Notes}");
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            }).GeneratePdf();

            return Result.Success(pdfBytes);
        }

        // Helper method
        private InvoiceDetailsDto ConvertToDetailsDto(Invoice invoice, Patient patient, Doctor doctor)
        {
            return new InvoiceDetailsDto
            {
                InvoiceId = invoice.Id,
                InvoiceDate = invoice.InvoiceDate,
                PaymentStatus = invoice.Status.ToString(),
                ClinicName = doctor.Clinic?.Name ?? string.Empty,
                Patient = new PatientResponseDto { Id = patient.PatientId, Name = patient.User?.FullName ?? string.Empty },
                Doctor = new DoctorResponseDto { Id = doctor.Id, Name = doctor.User?.FullName ?? string.Empty },
                AmountBreakdown = new AmountBreakdownDto
                {
                    ServiceCharge = invoice.TotalAmountEGP,
                    Discount = invoice.DiscountEGP,
                    Total = invoice.FinalAmountEGP,
                    PaidAmount = invoice.PaidAmountEGP,
                    Remaining = invoice.FinalAmountEGP - invoice.PaidAmountEGP
                },
                Notes = invoice.Notes
            };
        }

        public async Task<Result<InvoiceDetailsDto>> GetInvoiceDetailsAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Patient).ThenInclude(p => p.User)
                .Include(i => i.Doctor).ThenInclude(d => d.Clinic)
                .Include(i => i.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted, cancellationToken);

            if (invoice == null)
                return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.NotFound);

            return Result.Success(ConvertToDetailsDto(invoice, invoice.Patient, invoice.Doctor));
        }
    }
}
