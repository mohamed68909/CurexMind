namespace ClincManagement.API.Contracts.Doctors.Requests
{
    public class CreateDoctorRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string Languages { get; set; } = string.Empty;

    
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public IFormFile? ProfileImage { get; set; }


        public Guid ClinicId { get; set; }

    }

    public class UpdateDoctorRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string Languages { get; set; } = string.Empty;

        
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        
        public IFormFile? NewProfileImage { get; set; }

    
        public Guid ClinicId { get; set; }

    }
}
