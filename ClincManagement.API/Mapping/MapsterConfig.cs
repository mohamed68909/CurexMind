using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;
using ClincManagement.API.Contracts.Authentications.Requests;
using ClincManagement.API.Contracts.Clinic.Respones;
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
            config.NewConfig<SignUpRequest, ApplicationUser>();
                
               
            config.NewConfig<CreateRequestAppointment, Appointment>();
            config.NewConfig<Appointment, ResponseDetailsAppointment>();
            config.NewConfig<Doctor, ResponseDetailsDoctors>();
            config.NewConfig<Patient, PatientCreateResponseDto>();
            config.NewConfig<Patient, PagedPatientResponse>();
            config.NewConfig<Patient, PatientResponseDto>();
            config.NewConfig<PatientRequestDto, Patient>();


        }
    }
}
