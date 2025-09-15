using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;
using ClincManagement.API.Contracts.Authentications.Requests;
using ClincManagement.API.Contracts.Clinic.Respones;
using ClincManagement.API.Contracts.Doctors.Respones;
using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Contracts.Patient.Responses;
using Mapster;

namespace ClincManagement.API.Mapping
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // User mappings
            config.NewConfig<SignUpRequest, ApplicationUser>()
               .Map(dest => dest.EmailConfirmed, src => true);


               
            config.NewConfig<CreateRequestAppointment, Appointment>();
            config.NewConfig<Appointment, ResponseDetailsAppointment>();
            config.NewConfig<Doctor, DoctorListResponse>()
                .Map(dest => dest.ClinicName, src => src.Clinic.Name);






            config.NewConfig<Doctor, DoctorDetailsResponse>()
                .Map(dest => dest.ClinicName, src => src.Clinic.Name)
                .Map(dest => dest.Reviews, src => src.Reviews.Adapt<IEnumerable<ReviewResponse>>());

            config.NewConfig<PatientRequestDto, Patient>()
             .Ignore(dest => dest.PatientId)    
             .Ignore(dest => dest.CreatedDate)     
             .Map(dest => dest.User.FullName, src => src.FullName)
             .Map(dest => dest.Gender, src => src.Gender)
             .Map(dest => dest.DateOfBirth, src => src.DateOfBirth)
             .Map(dest => dest.SocialStatus, src => src.SocialStatus)
             .Map(dest => dest.User.FullName, src => src.PhoneNumber)
             .Map(dest => dest.User.Email, src => src.Email)
             .Map(dest => dest.NationalId, src => src.NationalId)
             .Map(dest => dest.User.Address, src => src.Address)
             .Map(dest => dest.Notes, src => src.Notes);

         
            config.NewConfig<InitialBooking, Appointment>()
                .Ignore(dest => dest.Id)
                .Map(dest => dest.ClinicId, src => src.ClinicId)
                .Map(dest => dest.DoctorId, src => src.DoctorId)
                .Map(dest => dest.Type, src => src.AppointmentType)
                .Map(dest => dest.AppointmentDate, src => src.AppointmentDate ?? DateTime.UtcNow)
                .Map(dest => dest.Notes, src => src.Notes);



            config.NewConfig<Patient, PatientCreateResponseDto>();
            config.NewConfig<Patient, PagedPatientResponse>();
            config.NewConfig<Patient, PatientResponseDto>();
            config.NewConfig<PatientRequestDto, Patient>();

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



        }
    }
}
