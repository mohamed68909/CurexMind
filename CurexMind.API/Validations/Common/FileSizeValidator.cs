using CurexMind.API.Settings;
using FluentValidation;

namespace CurexMind.API.Validations.Common
{
    public class FileSizeValidator : AbstractValidator<IFormFile>
    {
        public FileSizeValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("PDF file is required.");

            RuleFor(x => x)
                .Must((request, context) => request.Length <= FileSettings.InvoiceSettings.MaxSizeInBytes)
                .WithMessage($"PDF file size must not exceed {FileSettings.InvoiceSettings.MaxSizeInMB} MB.")
                .When(request => request is not null);
        }
    }
}
