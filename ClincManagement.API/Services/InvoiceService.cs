using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Invoice.Requests;
using ClincManagement.API.Contracts.Invoice.Responses;
using ClincManagement.API.Services.Interfaces;
using ClinicManagement.API.Errors;
using Microsoft.EntityFrameworkCore;

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
                IsDeleted = false,
                CreatedById= "System"

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
                .FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted, cancel);

            if (invoice == null)
                return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.NotFound);

            if (invoice.Status == InvoiceStatus.Paid)
                return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.AlreadyPaid);

            if (request.PatientId != null)
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == request.PatientId, cancel);
                if (patient == null)
                    return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.InvalidPatient);
                invoice.PatientId = request.PatientId;
            }

            if (request.DoctorId != null)
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

            invoice.UpdatedOn = DateTime.UtcNow;
            invoice.CreatedById ="St";

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
            invoice.UpdatedOn = DateTime.UtcNow;

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

                PatientName = i.Patient.User.FullName,
                DoctorName = i.Doctor.User.FullName,

                ServiceType = i.ServiceType.Name,

                TotalAmount = i.TotalAmountEGP,
                DiscountApplied = i.DiscountEGP,
                NetTotal = i.FinalAmountEGP,

                PaymentMethod = i.PaymentMethod,
                PaymentStatus = i.Status.ToString()
            });

            return Result.Success(list);
        }



        // GENERATE PDF (Stub)
        public async Task<Result<byte[]>> GeneratePdfExportAsync(Guid invoiceId, CancellationToken cancel = default)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted, cancel);

            if (invoice == null)
                return Result.Failure<byte[]>(InvoiceErrors.NotFound);

            // TODO: Implement PDF generation
            return Result.Success(Array.Empty<byte>());
        }

        // Helper method
        private InvoiceDetailsDto ConvertToDetailsDto(Invoice invoice, Patient patient, Doctor doctor)
        {
            return new InvoiceDetailsDto
            {
                InvoiceId = invoice.Id,
                InvoiceDate = invoice.InvoiceDate,
                PaymentStatus = invoice.Status.ToString(),
                ClinicName = doctor.Clinic.Name,
                Patient = new PatientResponseDto { Id = patient.PatientId, Name = patient.User.FullName },
                Doctor = new DoctorResponseDto { Id = doctor.Id, Name = doctor.User.FullName },
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

        public Task<Result<InvoiceDetailsDto>> GetInvoiceDetailsAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
