namespace ClincManagement.API.Contracts.Doctors.Respones
{
    public record DoctorDetailsResponse
    {
        public Guid Id { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string Specialization { get; init; } = string.Empty;
        public string ClinicName { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Languages { get; init; }
        public int YearsOfExperience { get; init; }
        public string Bio { get; init; } = string.Empty;
        public double Rating { get; init; }
        public int ReviewsCount { get; init; }
        public string ProfileImageUrl { get; init; } 
        public List<AppointmentResponse> Appointments { get; init; } = new();
        public List<ReviewResponse> Reviews { get; init; } = new();
    }

    public record AppointmentResponse
    {
        public string Id { get; init; } = string.Empty;
        public DateTime DateTime { get; init; }
        public bool IsBooked { get; init; }
    }

    public record ReviewResponse
    {
        public Guid Id { get; init; }
        public string PatientName { get; init; } = string.Empty;
        public double Rating { get; init; } = 5;
        public string Comment { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

}
