namespace ClincManagement.API.Contracts.Patient.Requests
{
    public record PatientRequestDto
    (

        string FullName,
        string UserName,

        Gender Gender,


        DateTime DateOfBirth,

        SocialStatus SocialStatus,

        IFormFile? ProfileImageUrl,

        string PhoneNumber,


        string? Email,


        string? NationalId,

        string? Address,

        string? Notes,

        InitialBooking? InitialBooking
    )
    {

    };

    public record InitialBooking
    (
        Guid ClinicId,
        Guid DoctorId,
        AppointmentType AppointmentType,
        DateTime? AppointmentDate,
        string? Notes
    );
}
