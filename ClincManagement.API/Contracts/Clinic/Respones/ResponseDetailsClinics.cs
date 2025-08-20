namespace ClincManagement.API.Contracts.Clinic.Respones
{
    public record ResponseDetailsClinics
    (
        Guid ClinicId,
        string name,
        string description
    );
}
