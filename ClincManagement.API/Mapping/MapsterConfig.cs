using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;
using ClincManagement.API.Contracts.Authentications.Requests;
using ClincManagement.API.Contracts.Clinic.Respones;
using ClincManagement.API.Contracts.Doctors.Respones;
using ClincManagement.API.Contracts.Invoice.Respones;
using ClincManagement.API.Contracts.Operation.Response;
using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Contracts.Patient.Responses;
using ClincManagement.API.Contracts.Stay.Requests;
using ClincManagement.API.Contracts.Stay.Respones;
using ClincManagement.API.Contracts.Stay.Responses;
using Mapster;
using PatientResponseDto = ClincManagement.API.Contracts.Patient.Respones.PatientResponseDto;

namespace ClincManagement.API.Mapping
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<SignUpRequest, ApplicationUser>()
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.FullName, src => src.FullName)
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.EmailConfirmed, _ => true);

            config.NewConfig<Appointment, AppointmentDto>()
                .Map(dest => dest.PatientName, src => src.Patient.User.FullName)
                .Map(dest => dest.DoctorName, src => src.Doctor.FullName)
                .Map(dest => dest.ClinicName, src => src.Clinic.Name);

            config.NewConfig<CreateAppointmentDto, Appointment>();
            config.NewConfig<UpdateAppointmentDto, Appointment>();
            config.NewConfig<Appointment, ResponseDetailsAllAppointment>();

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

            config.NewConfig<Clinic, DoctorListResponse>();
            config.NewConfig<Doctor, DoctorListResponse>()
                .Map(dest => dest.ClinicName, src => src.Clinic.Name)
                .Map(dest => dest.ProfileImageUrl, src => GenerateImageUrl(src.User.ProfileImageUrl.StoredFileName));

            config.NewConfig<Review, ReviewResponse>();

            config.NewConfig<Doctor, DoctorDetailsResponse>()
                .Map(dest => dest.ClinicName, src => src.Clinic.Name)
                .Map(dest => dest.Reviews, src => src.Reviews.Adapt<IEnumerable<ReviewResponse>>())
                .Map(dest => dest.ProfileImageUrl, src => GenerateImageUrl(src.User.ProfileImageUrl.StoredFileName));

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
            config.NewConfig<Patient, PatientResponseDto>()
                .Map(dest => dest.FullName, src => src.User.FullName)
                .Map(dest => dest.PhoneNumber, src => src.User.PhoneNumber)
                .Map(dest => dest.Email, src => src.User.Email)
                .Map(dest => dest.ProfileImageUrl, src => GenerateImageUrl(src.User.ProfileImageUrl.StoredFileName))
                .Map(dest => dest.Age, src => CalculateAge(src.DateOfBirth));

            config.NewConfig<Patient, PatientListResponseDto>()
                .Map(dest => dest.PatientId, src => src.PatientId)
                .Map(dest => dest.PatientId, src => $"#{src.PatientId.ToString().Substring(0, 4).ToUpper()}")
                .Map(dest => dest.FullName, src => src.User.FullName)
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.Age, src => CalculateAge(src.DateOfBirth))
                .Map(dest => dest.PhoneNumber, src => src.User.PhoneNumber)
                .Map(dest => dest.Address, src => src.Address);


            config.NewConfig<CreateStayDto, Stay>()
                .Map(dest => dest.CheckInDate, src => src.CheckInDate);

            config.NewConfig<UpdateStayDto, Stay>()
                .Map(dest => dest.CheckOutDate, src => src.CheckOutDate);

            config.NewConfig<Stay, StayDto>()
                .Map(dest => dest.PatientName, src => src.Patient.User.FullName)
                .Map(dest => dest.RoomNumber, src => src.RoomNumber)
                .Map(dest => dest.BedNumber, src => src.BedNumber)
                .Map(dest => dest.ActivityLog, src => src.ActivityLog.Adapt<List<ActivityLogDto>>());

            config.NewConfig<StayActivity, ActivityLogDto>();

            config.NewConfig<Stay, ResponsePatientStay>()
                .Map(dest => dest.RoomBed, src => $"{src.RoomNumber}/{src.BedNumber}")
                .Map(dest => dest.CheckIn, src => src.CheckInDate.ToString("yyyy-MM-dd HH:mm"))
                .Map(dest => dest.CheckOut, src => src.CheckOutDate.HasValue ? src.CheckOutDate.Value.ToString("yyyy-MM-dd HH:mm") : "N/A")
            .Map(dest => dest.Services, src => src.ServiceType.ToString())
                .Map(dest => dest.TotalCost, src => src.TotalCost);

            config.NewConfig<Invoice, ResponsePatientInvoice>()
          .Map(dest => dest.Date, src => src.InvoiceDate.ToString("yyyy-MM-dd"))
          .Map(dest => dest.InvoiceNumber, src => src.InvoiceNumber)

          .Map(dest => dest.Amount, src => src.FinalAmountEGP)
          .Map(dest => dest.Paid, src => src.PaidAmountEGP)
          .Map(dest => dest.Remaining, src => src.FinalAmountEGP - src.PaidAmountEGP)
          .Map(dest => dest.Status, src => src.Status.ToString());

            config.NewConfig<Operation, ResponsePatientOperation>()
                .Map(dest => dest.Operation, src => src.Name)
                .Map(dest => dest.Date, src => src.Date.ToString("yyyy-MM-dd"))
                .Map(dest => dest.Surgeon, src => src.Doctor.FullName)
                .Map(dest => dest.ToolsItems, src => src.Tools)
                .Map(dest => dest.CostNotes, src => $"{src.Cost:C} - {src.Notes}");
        }

        private static string GenerateImageUrl(string? storedFileName)
        {
            return string.IsNullOrWhiteSpace(storedFileName)
                ? string.Empty
                : $"/images/{storedFileName}";
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}