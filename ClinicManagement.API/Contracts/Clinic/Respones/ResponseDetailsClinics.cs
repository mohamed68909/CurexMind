namespace ClinicManagement.API.Contracts.Clinic.Respones
{
    public record ResponseDetailsClinics
    (
        Guid ClinicId,
        string name,
        string description
    );
}
