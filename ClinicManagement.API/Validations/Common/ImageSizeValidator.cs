using ClinicManagement.API.Settings;
using FluentValidation;

namespace ClinicManagement.API.Validations.Common
{
    public class ImageSizeValidator : AbstractValidator<IFormFile>
    {
        public ImageSizeValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("Cover image is required.")
                .Must((request, context) => request.Length <= FileSettings.ImageSettings.MaxSizeInBytes)
                .WithMessage($"Cover image size must not exceed {FileSettings.ImageSettings.MaxSizeInMB} MB.")
                .When(c => c is not null);
        }
    }
}
