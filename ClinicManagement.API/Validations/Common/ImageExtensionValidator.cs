using ClinicManagement.API.Settings;
using FluentValidation;

namespace ClinicManagement.API.Validations.Common
{
    public class ImageExtensionValidator : AbstractValidator<IFormFile>
    {
        public ImageExtensionValidator()
        {
            RuleFor(x => x)
              .Must(file =>
              {
                  // Fix: use 'using var' to ensure the stream is always disposed (stream leak fix)
                  using var binary = new BinaryReader(file.OpenReadStream());
                  var bytes = binary.ReadBytes(2); // global signature (magic bytes)
                  var fileSequenceHex = BitConverter.ToString(bytes);

                  return FileSettings.ImageSettings.AllowedSignatures
                      .Any(sig => sig.Equals(fileSequenceHex, StringComparison.OrdinalIgnoreCase));
              })
              .WithMessage("Cover image must be a valid PNG, JPEG, GIF, or BMP file.")
              .When(file => file is not null);
        }
    }
}
