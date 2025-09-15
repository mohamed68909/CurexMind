using System.ComponentModel.DataAnnotations;

namespace ClincManagement.API.Contracts.Patient.Requests
{
    public record PatientRequestDto
    (
        [Required, MaxLength(255)]
        string FullName,

        [Required]
        Gender Gender, 

        [Required]
        DateTime DateOfBirth,

        SocialStatus? SocialStatus,

  
        string PhoneNumber,

        
        string? Email,

        [MaxLength(50)]
        string? NationalId,

        [MaxLength(500)]
        string? Address,

        string? Notes,

        InitialBooking? InitialBooking
    );

    public record InitialBooking
    (
        Guid? ClinicId,
        Guid? DoctorId,
        AppointmentType? AppointmentType,
        DateTime? AppointmentDate,
        string? Notes
    );
}
