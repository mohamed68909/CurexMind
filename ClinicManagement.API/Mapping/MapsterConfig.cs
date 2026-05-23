using ClinicManagement.API.Contracts.Appinments.Requests;
using ClinicManagement.API.Contracts.Appinments.Respones;
using ClinicManagement.API.Contracts.Authentications.Requests;
using ClinicManagement.API.Contracts.Clinic.Respones;

using ClinicManagement.API.Contracts.Operation.Response;
using ClinicManagement.API.Contracts.Patient.Requests;
using ClinicManagement.API.Contracts.Stay.Requests;


using Mapster;

namespace ClinicManagement.API.Mapping
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

          
           

        }
    }
}
