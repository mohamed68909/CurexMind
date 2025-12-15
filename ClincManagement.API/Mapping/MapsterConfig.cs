using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;
using ClincManagement.API.Contracts.Authentications.Requests;
using ClincManagement.API.Contracts.Clinic.Respones;

using ClincManagement.API.Contracts.Operation.Response;
using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Stay.Requests;


using Mapster;

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

            //config.NewConfig<Appointment, AppointmentDto>()
            //    .Map(dest => dest.PatientName, src => src.Patient.User.FullName)
            //    .Map(dest => dest.DoctorName, src => src.Doctor.User.FullName)
            //    .Map(dest => dest.ClinicName, src => src.Clinic.Name);

            //config.NewConfig<CreateAppointmentDto, Appointment>();
            //config.NewConfig<UpdateAppointmentDto, Appointment>();
            //config.NewConfig<Appointment, ResponseDetailsAllAppointment>();

            //config.NewConfig<InitialBooking, Appointment>()
            //    .Ignore(dest => dest.Id)
            //    .Map(dest => dest.ClinicId, src => src.ClinicId)
            //    .Map(dest => dest.DoctorId, src => src.DoctorId)
            //    .Map(dest => dest.Type, src => src.AppointmentType)
            //    .Map(dest => dest.AppointmentDate, src => src.AppointmentDate ?? DateTime.UtcNow)
            //    .Map(dest => dest.Notes, src => src.Notes);

            //config.NewConfig<Clinic, ClinicResponse>();

            //config.NewConfig<CreateStayDto, Stay>()
            //    .Map(dest => dest.CheckInDate, src => src.CheckInDate);

            //config.NewConfig<UpdateStayDto, Stay>()
            //    .Map(dest => dest.CheckOutDate, src => src.CheckOutDate);

            //config.NewConfig<Stay, StayDto>()
            //    .Map(dest => dest.PatientName, src => src.Patient.User.FullName)
            //    .Map(dest => dest.RoomNumber, src => src.RoomNumber)
            //    .Map(dest => dest.BedNumber, src => src.BedNumber)
            //    .Map(dest => dest.ActivityLog, src => src.ActivityLog.Adapt<List<ActivityLogDto>>());

            //config.NewConfig<StayActivity, ActivityLogDto>();

            //config.NewConfig<Stay, ResponsePatientStay>()
            //    .Map(dest => dest.RoomBed, src => $"{src.RoomNumber}/{src.BedNumber}")
            //    .Map(dest => dest.CheckIn, src => src.CheckInDate.ToString("yyyy-MM-dd HH:mm"))
            //    .Map(dest => dest.CheckOut, src =>
            //        src.CheckOutDate.HasValue
            //            ? src.CheckOutDate.Value.ToString("yyyy-MM-dd HH:mm")
            //            : "N/A")
            //    .Map(dest => dest.Services, src => src.ServiceType.ToString())
            //    .Map(dest => dest.TotalCost, src => src.TotalCost);

            //config.NewConfig<Invoice, ResponsePatientInvoice>()
            //    .Map(dest => dest.Date, src => src.InvoiceDate.ToString("yyyy-MM-dd"))
            //    .Map(dest => dest.InvoiceNumber, src => src.InvoiceNumber)
            //    .Map(dest => dest.Amount, src => src.FinalAmountEGP)
            //    .Map(dest => dest.Paid, src => src.PaidAmountEGP)
            //    .Map(dest => dest.Remaining, src => src.FinalAmountEGP - src.PaidAmountEGP)
            //    .Map(dest => dest.Status, src => src.Status.ToString());

            config.NewConfig<Operation, ResponsePatientOperation>()
                .Map(dest => dest.Operation, src => src.Name)
                .Map(dest => dest.Date, src => src.Date.ToString("yyyy-MM-dd"))
                .Map(dest => dest.Surgeon, src => src.Doctor.User.FullName)
                .Map(dest => dest.ToolsItems, src => src.Tools)
                .Map(dest => dest.CostNotes, src => $"{src.Cost:C} - {src.Notes}");
        }
    }
}
