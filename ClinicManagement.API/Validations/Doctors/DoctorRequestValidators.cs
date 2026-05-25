using ClinicManagement.API.Contracts.Doctors.Requests;
using ClinicManagement.API.Settings;
using FluentValidation;

namespace ClinicManagement.API.Validations.Doctors
{
    public class CreateDoctorRequestValidator : AbstractValidator<CreateDoctorRequest>
    {
        public CreateDoctorRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.Specialization)
                .NotEmpty().WithMessage("Specialization is required.");

            RuleFor(x => x.ClinicId)
                .NotEmpty().WithMessage("Clinic is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

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

    public class UpdateDoctorRequestValidator : AbstractValidator<UpdateDoctorRequest>
    {
        public UpdateDoctorRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.");

            RuleFor(x => x.Specialization)
                .NotEmpty().WithMessage("Specialization is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            // Validate new profile image only if provided
            When(x => x.NewProfileImage != null, () =>
            {
                RuleFor(x => x.NewProfileImage!)
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
