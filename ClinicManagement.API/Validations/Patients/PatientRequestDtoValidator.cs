using ClinicManagement.API.Contracts.Patient.Requests;
using ClinicManagement.API.Settings;
using FluentValidation;

namespace ClinicManagement.API.Validations.Patients
{
    public class PatientRequestDtoValidator : AbstractValidator<PatientRequestDto>
    {
        public PatientRequestDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.");

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("National ID is required.")
                .Length(14).WithMessage("National ID must be exactly 14 digits.")
                .Matches("^[0-9]+$").WithMessage("National ID must contain digits only.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past.")
                .GreaterThan(DateTime.UtcNow.AddYears(-120)).WithMessage("Date of birth is not valid.");

            // Validate profile image only if provided
            When(x => x.ProfileImage != null, () =>
            {
                RuleFor(x => x.ProfileImage!)
                    .Must(f => f.Length <= FileSettings.ImageSettings.MaxSizeInBytes)
                    .WithMessage($"Profile image must not exceed {FileSettings.ImageSettings.MaxSizeInMB} MB.")
                    .Must(f =>
                    {
                        using var binary = new BinaryReader(f.OpenReadStream());
                        var sig = BitConverter.ToString(binary.ReadBytes(2));
                        return FileSettings.ImageSettings.AllowedSignatures
                            .Any(s => s.Equals(sig, StringComparison.OrdinalIgnoreCase));
                    })
                    .WithMessage("Profile image must be a valid PNG, JPEG, GIF, or BMP file.");
            });
        }
    }
}
