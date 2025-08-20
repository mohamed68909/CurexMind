namespace ClincManagement.API.Contracts.Patient.Respones
{
    public record ResponsePatientsCreated
   (
        Guid PatientId,
        string patientNumber,
        string message = "Patient created successfully."



        );
}
