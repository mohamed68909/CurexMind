using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Invoice.Requests;
using ClincManagement.API.Contracts.Invoice.Requests.ClinicManagement.API.Services.Dtos;
using ClincManagement.API.Contracts.Invoice.Respones;
using ClincManagement.API.Contracts.Invoice.Respones.ClinicManagement.API.Services.Dtos;
using ClinicManagement.API.Errors;
using ClinicManagement.API.Services.Interface;

namespace ClinicManagement.API.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;
        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<InvoiceDetailsDto>> CreateInvoiceAsync(CreateInvoiceDto request, CancellationToken cancel)
        {
            var patient = await _context.Patients.FindAsync(request.PatientId);
            var doctor = await _context.Doctors.Include(d => d.Clinic).FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancel);
            var serviceType = await _context.ServiceTypes.FindAsync(request.ServiceDetails.ServiceTypeId);

            if (patient == null) return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.InvalidPatient);
            if (doctor == null) return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.InvalidDoctor);
            if (serviceType == null) return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.ServiceMismatch);

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceDate = DateTime.UtcNow,
                PatientId = request.PatientId,
                DoctorId = doctor.Id,
                ClinicId = doctor.ClinicId,

                ServiceTypeId = request.ServiceDetails.ServiceTypeId,
                VisitDate = request.ServiceDetails.VisitDate,
                VisitTime = request.ServiceDetails.VisitDate.ToShortTimeString(),
                TotalAmountEGP = request.AmountDetails.TotalAmountEGP,
                DiscountEGP = request.AmountDetails.DiscountEGP,
                FinalAmountEGP = request.AmountDetails.FinalAmountEGP,

                PaymentMethod = request.PaymentInformation.PaymentMethod,
                Status = Enum.Parse<InvoiceStatus>(request.PaymentInformation.PaymentStatus),
                PaidAmountEGP = request.PaymentInformation.AmountPaidEGP,

                Notes = request.Notes,
                IsDeleted = false
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync(cancel);

            invoice.ServiceType = serviceType;
            invoice.Patient = patient;
            invoice.Doctor = doctor;

            var responseDto = new InvoiceDetailsDto
            {
                InvoiceId = invoice.Id,
                InvoiceDate = invoice.InvoiceDate,
                PaymentStatus = invoice.Status.ToString(),
                ClinicName = doctor.Clinic.Name,

                Patient = new PatientResponseDto { Id = patient.PatientId, Name = patient.User.FullName },
                Doctor = new DoctorResponseDto { Id = doctor.Id, Name = doctor.FullName },
                Service = new ServiceResponseDto { Type = invoice.ServiceType.Name, VisitDate = invoice.VisitDate, VisitTime = invoice.VisitTime },
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

            return Result.Success(responseDto);
        }

        public async Task<Result<InvoiceDetailsDto>> UpdateInvoiceAsync(string invoiceId, UpdateInvoiceDto request, CancellationToken cancel)
        {
            var invoiceGuid = Guid.Parse(invoiceId);
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceGuid && !i.IsDeleted, cancel);

            if (invoice == null) return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.NotFound);

            if (request.PatientId != null) invoice.PatientId = request.PatientId;
            if (request.DoctorId != null) invoice.DoctorId = request.DoctorId;

            if (request.AmountDetails != null)
            {
                invoice.TotalAmountEGP = request.AmountDetails.TotalAmountEGP;
                invoice.DiscountEGP = request.AmountDetails.DiscountEGP;
                invoice.FinalAmountEGP = request.AmountDetails.FinalAmountEGP;
            }

            if (request.PaymentInformation != null)
            {
                invoice.PaymentMethod = request.PaymentInformation.PaymentMethod;
                invoice.Status = Enum.Parse<InvoiceStatus>(request.PaymentInformation.PaymentStatus);

                invoice.PaidAmountEGP = request.PaymentInformation.AmountPaidEGP;
            }


            invoice.UpdatedOn = DateTime.UtcNow;

            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync(cancel);


            var updatedInvoice = await _context.Invoices
                .Where(i => i.Id == invoiceGuid && !i.IsDeleted)
                .Include(i => i.Patient).ThenInclude(p => p.User)
                .Include(i => i.Doctor).ThenInclude(d => d.Clinic)
                .Include(i => i.ServiceType)
                .FirstOrDefaultAsync(cancel);

            var updatedDto = new InvoiceDetailsDto
            {
                InvoiceId = updatedInvoice.Id,
                InvoiceDate = updatedInvoice.InvoiceDate,
                PaymentStatus = updatedInvoice.Status.ToString(),
                ClinicName = updatedInvoice.Doctor.Clinic.Name,
                Patient = new PatientResponseDto { Id = updatedInvoice.Patient.PatientId, Name = updatedInvoice.Patient.User.FullName },
                Doctor = new DoctorResponseDto { Id = updatedInvoice.Doctor.Id, Name = updatedInvoice.Doctor.FullName },
                Service = new ServiceResponseDto { Type = updatedInvoice.ServiceType.Name, VisitDate = updatedInvoice.VisitDate, VisitTime = updatedInvoice.VisitTime },
                AmountBreakdown = new AmountBreakdownDto
                {
                    ServiceCharge = updatedInvoice.TotalAmountEGP,
                    Discount = updatedInvoice.DiscountEGP,
                    Total = updatedInvoice.FinalAmountEGP,
                    PaidAmount = updatedInvoice.PaidAmountEGP,
                    Remaining = updatedInvoice.FinalAmountEGP - updatedInvoice.PaidAmountEGP
                },
                Notes = updatedInvoice.Notes
            };

            return Result.Success(updatedDto);
        }

        public async Task<Result> DeleteInvoiceAsync(string invoiceId)
        {
            var invoiceGuid = Guid.Parse(invoiceId);
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceGuid && !i.IsDeleted);

            if (invoice == null) return Result.Failure(InvoiceErrors.NotFound);

            invoice.IsDeleted = true;

            invoice.UpdatedOn = DateTime.UtcNow;

            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            return Result.Success();
        }


        public async Task<Result<IEnumerable<InvoiceSummaryDto>>> GetAllInvoicesAsync(InvoiceFilterDto filterParams = null)
        {
            filterParams ??= new InvoiceFilterDto();

            var query = _context.Invoices
                .Where(i => !i.IsDeleted)
                .Include(i => i.Patient).ThenInclude(p => p.User)
                .Include(i => i.Doctor)
                .AsQueryable();

            if (filterParams != null)
            {
                if (!string.IsNullOrWhiteSpace(filterParams.SearchQuery))
                {
                    query = query.Where(i =>
                        i.Id.ToString().Contains(filterParams.SearchQuery) ||
                        i.Patient.User.FullName.Contains(filterParams.SearchQuery) ||
                        i.Doctor.FullName.Contains(filterParams.SearchQuery));
                }

                if (!string.IsNullOrWhiteSpace(filterParams.PaymentStatus))
                {
                    var status = Enum.Parse<InvoiceStatus>(filterParams.PaymentStatus);
                    query = query.Where(i => i.Status == status);
                }
            }

            query = ApplySorting(query, filterParams);

            var totalCount = await query.CountAsync();
            if (totalCount == 0) return Result.Failure<IEnumerable<InvoiceSummaryDto>>(InvoiceErrors.NotFound);

            var pagedQuery = query.Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
                                  .Take(filterParams.PageSize);

            var invoices = await pagedQuery.ToListAsync();

            var data = invoices.Select(i => new InvoiceSummaryDto
            {
                InvoiceId = i.Id.ToString(),
                InvoiceDate = i.InvoiceDate,
                PatientName = i.Patient.User.FullName,
                DoctorName = i.Doctor.FullName,
                TotalAmount = i.FinalAmountEGP,
                PaymentStatus = i.Status.ToString()
            }).ToList();

            return Result.Success((IEnumerable<InvoiceSummaryDto>)data);
        }

        public async Task<Result<InvoiceDetailsDto>> GetInvoiceDetailsAsync(string invoiceId)
        {
            var invoiceGuid = Guid.Parse(invoiceId);
            var invoice = await _context.Invoices
                .Where(i => i.Id == invoiceGuid && !i.IsDeleted)
                .Include(i => i.Patient).ThenInclude(p => p.User)
                .Include(i => i.Doctor).ThenInclude(d => d.Clinic)
                .Include(i => i.ServiceType)
                .FirstOrDefaultAsync();

            if (invoice == null) return Result.Failure<InvoiceDetailsDto>(InvoiceErrors.NotFound);

            var responseDto = new InvoiceDetailsDto
            {
                InvoiceId = invoice.Id,
                InvoiceDate = invoice.InvoiceDate,
                PaymentStatus = invoice.Status.ToString(),
                ClinicName = invoice.Doctor.Clinic.Name,
                Patient = new PatientResponseDto { Id = invoice.Patient.PatientId, Name = invoice.Patient.User.FullName },
                Doctor = new DoctorResponseDto { Id = invoice.Doctor.Id, Name = invoice.Doctor.FullName },
                Service = new ServiceResponseDto { Type = invoice.ServiceType.Name, VisitDate = invoice.VisitDate, VisitTime = invoice.VisitTime },
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

            return Result.Success(responseDto);
        }

        public async Task<Result<byte[]>> GeneratePdfExportAsync(string invoiceId)
        {
            var invoiceGuid = Guid.Parse(invoiceId);
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceGuid && !i.IsDeleted);

            if (invoice == null) return Result.Failure<byte[]>(InvoiceErrors.NotFound);

            byte[] pdfBytes = Array.Empty<byte>();

            return Result.Success(pdfBytes);
        }

        private IQueryable<Invoice> ApplySorting(IQueryable<Invoice> query, InvoiceFilterDto filter)
        {
            return filter.SortBy.ToLower() switch
            {
                "invoicedate" when filter.SortDirection.ToLower() == "asc" => query.OrderBy(i => i.InvoiceDate),
                "invoicedate" => query.OrderByDescending(i => i.InvoiceDate),
                "totalamount" when filter.SortDirection.ToLower() == "asc" => query.OrderBy(i => i.FinalAmountEGP),
                "totalamount" => query.OrderByDescending(i => i.FinalAmountEGP),
                _ => query.OrderByDescending(i => i.InvoiceDate)
            };
        }
    }
}