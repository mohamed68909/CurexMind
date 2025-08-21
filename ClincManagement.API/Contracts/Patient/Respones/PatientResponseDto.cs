namespace ClincManagement.API.Contracts.Patient.Respones
{
    public record PatientResponseDto
    (
        Guid PatientId,
        string FullName,
        Gender Gender,
  
        int Age,
        SocialStatus? SocialStatus,
        string PhoneNumber,
        string? Email,
        string? NationalId,
        string? Address,
        string? Notes,
        string? ProfileImageUrl,
       // VitalSignsDto? VitalSigns,
        DateTime CreatedDate
    );

    //public record VitalSignsDto
    //(
    //    string? BloodPressure,
    //    int? HeartRate,
    //    double? Temperature,
    //    double? Weight
    //);

}
