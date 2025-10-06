using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;
using ClincManagement.API.Contracts.Authentications.Requests;
using ClincManagement.API.Contracts.Clinic.Respones;
using ClincManagement.API.Contracts.Doctors.Respones;
using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Contracts.Patient.Responses;
using ClincManagement.API.Contracts.Stay.Requests;
using ClincManagement.API.Contracts.Stay.Respones;
using ClincManagement.API.Contracts.Payment.Requests;
using ClincManagement.API.Contracts.Payment.Respones;
using ClincManagement.API.Entities;
using Mapster;

namespace ClincManagement.API.Mapping
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // ============================
            // ApplicationUser (Identity)
            // ============================
            config.NewConfig<SignUpRequest, ApplicationUser>()
                 .Map(dest => dest.Email, src => src.Email)
                 .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                 .Map(dest => dest.FullName, src => src.FullName)
                 .Map(dest => dest.UserName, src => src.UserName)
                 .Map(dest => dest.EmailConfirmed, _ => true);

            // ============================
            // Appointment
            // ============================
            config.NewConfig<Appointment, AppointmentDto>()
                 .Map(dest => dest.PatientName, src => src.Patient.User.FullName)
                 .Map(dest => dest.DoctorName, src => src.Doctor.FullName)
                 .Map(dest => dest.ClinicName, src => src.Clinic.Name);

            config.NewConfig<CreateAppointmentDto, Appointment>();
            config.NewConfig<UpdateAppointmentDto, Appointment>();

            config.NewConfig<Appointment, ResponseDetailsAppointment>();

            config.NewConfig<InitialBooking, Appointment>()
                 .Ignore(dest => dest.Id)
                 .Map(dest => dest.ClinicId, src => src.ClinicId)
                 .Map(dest => dest.DoctorId, src => src.DoctorId)
                 .Map(dest => dest.Type, src => src.AppointmentType)
                 .Map(dest => dest.AppointmentDate, src => src.AppointmentDate ?? DateTime.UtcNow)
                 .Map(dest => dest.Notes, src => src.Notes);

            TypeAdapterConfig<Appointment, ResponseAllAppointmentPatient>
                .NewConfig()
                .Map(dest => dest.AppointmentId, src => src.Id)
                .Map(dest => dest.DoctorName, src => src.Doctor.FullName)
                .Map(dest => dest.Specialization, src => src.Doctor.Specialization)
                .Map(dest => dest.Date, src => src.AppointmentDate)
                .Map(dest => dest.Time, src => src.AppointmentTime)
                .Map(dest => dest.VisitType, src => src.Type.ToString())
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(dest => dest.PaymentStatus,
                    src => src.Patient.Invoice != null && src.Patient.Invoice.Any(i => i.Status == InvoiceStatus.Paid)
                        ? "مدفوع"
                        : "غير مدفوع")
                .Map(dest => dest.InvoiceUrl,
                    src => src.Patient.Invoice != null && src.Patient.Invoice.Any()
                        ? $"/api/invoices/{src.Patient.Invoice.First().Id}"
                        : null);

            // ============================
            // Payment
            // ============================

            config.NewConfig<InitiatePaymentRequest, Payment>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Invoice)
                .Ignore(dest => dest.Appointment)
                .Ignore(dest => dest.PatientId)
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.Method, src => src.PaymentMethod)
                .Map(dest => dest.TransactionId, src => (string)null)
                .Map(dest => dest.Status, src => "Pending")
                .Map(dest => dest.CreatedAt, _ => DateTime.UtcNow);


            config.NewConfig<Payment, PaymentTransactionResponse>()
                .Map(dest => dest.PaymentId, src => src.Id)
                .Map(dest => dest.PaymentMethod, src => src.Method)
                .Map(dest => dest.Status, src => src.Status)
                .Map(dest => dest.TransactionReference, src => src.TransactionId)
                .Map(dest => dest.Message, src => $"Transaction status is: {src.Status}");

            // خريطة ResponseConfirmPyment تحتاج إلى خريطة PatientDetalis
            config.NewConfig<Payment, ResponseConfirmPyment>()
                .Map(dest => dest.PaymentId, src => src.Id)
                .Map(dest => dest.PaymentMethod, src => src.Method)
                .Map(dest => dest.Status, src => src.Status)
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.Message, _ => "Payment confirmed successfully.")

                .Map(dest => dest.doctorName, src => src.Appointment.Doctor.FullName)
                .Map(dest => dest.specialty, src => src.Appointment.Doctor.Specialization)
                // استخدام خريطة PatientDetalis
                .Map(dest => dest.PatientDetalis, src => src.Appointment.Patient.User.Adapt<PatientDetalis>());

            // إضافة خريطة PatientDetalis
            config.NewConfig<ApplicationUser, PatientDetalis>();


            // ============================
            // Doctor
            // ============================
            // إضافة خريطة Clinic لـ DoctorListResponse
            config.NewConfig<Clinic, DoctorListResponse>();

            config.NewConfig<Doctor, DoctorListResponse>()
                 .Map(dest => dest.ClinicName, src => src.Clinic.Name)
                 .Map(dest => dest.ProfileImageUrl, src => GenerateImageUrl(src.User.ProfileImageUrl.StoredFileName));

            // إضافة خريطة ReviewResponse
            config.NewConfig<Review, ReviewResponse>();

            config.NewConfig<Doctor, DoctorDetailsResponse>()
                 .Map(dest => dest.ClinicName, src => src.Clinic.Name)
                 .Map(dest => dest.Reviews, src => src.Reviews.Adapt<IEnumerable<ReviewResponse>>())
                 .Map(dest => dest.ProfileImageUrl, src => GenerateImageUrl(src.User.ProfileImageUrl.StoredFileName));

            // ============================
            // Patient
            // ============================
            config.NewConfig<PatientRequestDto, Patient>()
                 .Ignore(dest => dest.PatientId)
                 .Ignore(dest => dest.CreatedDate)
                 .Map(dest => dest.User.FullName, src => src.FullName)
                 .Map(dest => dest.Gender, src => src.Gender)
                 .Map(dest => dest.DateOfBirth, src => src.DateOfBirth)
                 .Map(dest => dest.SocialStatus, src => src.SocialStatus)
                 .Map(dest => dest.NationalId, src => src.NationalId)
                 .Map(dest => dest.Notes, src => src.Notes)
                 .Map(dest => dest.Address, src => src.Address)
                 .Map(dest => dest.User.ProfileImageUrl.StoredFileName, src => src.ProfileImageUrl.FileName);

            config.NewConfig<Patient, PatientCreateResponseDto>();
            config.NewConfig<Patient, PagedPatientResponse>();
            config.NewConfig<Patient, PatientResponseDto>();

            // ============================
            // Stay
            // ============================
            config.NewConfig<CreateStayDto, Stay>()
                 .Map(dest => dest.CheckInDate, src => src.CheckInDate);

            config.NewConfig<UpdateStayDto, Stay>()
                 .Map(dest => dest.CheckOutDate, src => src.CheckOutDate);


            config.NewConfig<Stay, StayDto>()
                 .Map(dest => dest.PatientName, src => src.Patient.User.FullName)
                 .Map(dest => dest.BedNumber, src => $"{src.RoomNumber} / {src.BedNumber}")
                 .Map(dest => dest.ActivityLog, src => src.ActivityLog.Adapt<List<ActivityLogDto>>());

            // Map StayActivity entity to ActivityLogDto
            config.NewConfig<StayActivity, ActivityLogDto>();
        }

        private static string GenerateImageUrl(string storedFileName)
        {
            if (string.IsNullOrWhiteSpace(storedFileName))
                return string.Empty;

            return $"/images/{storedFileName}";
        }
    }
}