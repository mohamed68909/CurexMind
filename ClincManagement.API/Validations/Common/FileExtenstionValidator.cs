using FluentValidation;
using static ClincManagement.API.Settings.FileSettings;

namespace ClincManagement.API.Validations.Common
{
    public class FileExtenstionValidator : AbstractValidator<IFormFile>
    {
        public FileExtenstionValidator()
        {
            RuleFor(x => x)
          .Must((request, context) =>
          {
              using var binary = new BinaryReader(request.OpenReadStream());
              var bytes = binary.ReadBytes(4);
              var fileSignature = BitConverter.ToString(bytes);
              return fileSignature.Equals(InvoiceSettings.AllowedSignuture, StringComparison.OrdinalIgnoreCase);
          })
          .WithMessage("Invalid PDF file format.")
          .When(x => x is not null);
        }
    }
}
