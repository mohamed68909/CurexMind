namespace ClincManagement.API.Entities
{
    [Owned]
    public class UploadedFile
    {
        public string FileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty; 
        public string ContentType { get; set; } = string.Empty; 
        public string FileExtension { get; set; } = string.Empty;
    }
}
