using CurexMind.API.Contracts.Appinments.Requests;
using CurexMind.API.Contracts.Appinments.Respones;
using CurexMind.API.Contracts.Authentications.Requests;
using CurexMind.API.Contracts.Clinic.Respones;

using CurexMind.API.Contracts.Operation.Response;
using CurexMind.API.Contracts.Patient.Requests;
using CurexMind.API.Contracts.Stay.Requests;


using Mapster;

namespace CurexMind.API.Mapping
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
