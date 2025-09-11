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

            config.NewConfig<Patient, PatientCreateResponseDto>();
            config.NewConfig<Patient, PagedPatientResponse>();
            config.NewConfig<Patient, PatientResponseDto>();
            config.NewConfig<PatientRequestDto, Patient>();


        }
    }
}
