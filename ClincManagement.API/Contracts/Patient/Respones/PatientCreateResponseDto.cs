namespace ClincManagement.API.Contracts.Patient.Respones
{
    public record PatientCreateResponseDto
   (
        Guid PatientId,

        string message = "Patient created successfully."



        );
}
