namespace ClincManagement.API.Services.Interface
{
    public interface IImageFileService
    {
        Task<UploadedFile> UploadAsync(IFormFile file, string folder);
        void Delete(UploadedFile file, string folder);
    }
}
