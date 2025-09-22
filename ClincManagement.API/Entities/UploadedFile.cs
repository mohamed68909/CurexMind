namespace ClincManagement.API.Entities
{
    [Owned]
    public class UploadedFile
    {
        public string FileName { get; set; } = string.Empty; // the origin file name
        public string StoredFileName { get; set; } = string.Empty; // fake name for Stored at database
        public string ContentType { get; set; } = string.Empty; // type of file like .pdf || .jpag..
        public string FileExtension { get; set; } = string.Empty;
    }
}
