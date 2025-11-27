using ClincManagement.API.Services.Interface;
using ClincManagement.API.Settings;

namespace ClincManagement.API.Services
{
    public class ImageFileService : IImageFileService
    {
        private readonly IWebHostEnvironment _env;

        public ImageFileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<UploadedFile> UploadAsync(IFormFile file, string folder)
        {
            if (file is null || file.Length == 0)
                throw new ArgumentException("No file provided.");

            if (file.Length > FileSettings.ImageSettings.MaxSizeInBytes)
                throw new ArgumentException($"Image size cannot exceed {FileSettings.ImageSettings.MaxSizeInMB} MB.");

          
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var bytes = ms.ToArray();
            var signature = BitConverter.ToString(bytes.Take(2).ToArray());

            if (!FileSettings.ImageSettings.AllowedSignatures.Any(s => s.StartsWith(signature)))
                throw new ArgumentException("Invalid image format (signature mismatch).");

        
            var uploadsFolder = Path.Combine(_env.WebRootPath, folder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, storedFileName);

            ms.Position = 0;
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ms.CopyToAsync(stream);
            }

         
            var uploadedFile = new UploadedFile
            {
                FileName = file.FileName,
                StoredFileName = storedFileName,
                ContentType = file.ContentType,
                FileExtension = Path.GetExtension(file.FileName)
            };

            return uploadedFile;
        }


        public void Delete(UploadedFile file, string folder)
        {
            if (file == null) return;

            var path = Path.Combine(_env.WebRootPath, folder, file.StoredFileName);
            if (File.Exists(path))
                File.Delete(path);
        }

    }
}

