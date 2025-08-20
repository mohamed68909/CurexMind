namespace ClincManagement.API.Contracts.Clinic.Respones
{
    public record ResponseDetailsDoctors
      (
        Guid DoctorId,
        string FullName,
        string specialization
    );
}
