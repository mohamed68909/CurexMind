using System.ComponentModel.DataAnnotations;
using static Azure.Core.HttpHeader;

namespace ClincManagement.API.Contracts.Patient.Requests
{
    public record RequestPatinents
    (
         string fullName,
         DateTime dateOfBirth,
         [Phone(ErrorMessage = "Invalid phone number format.")]
         string PhoneNumber,
            [EmailAddress(ErrorMessage = "Invalid email address format.")]
            string email,
            string? nationalId ,
            string? address ,
            IEnumerable<initialBooking>? allergies = null



        );
    public record initialBooking
        (
       Guid? ClinicId,
         Guid? DoctorId,
            DateTime? appointmentDate,
           
            DateTime? appointmentTime,
           string? notes


    );
}
