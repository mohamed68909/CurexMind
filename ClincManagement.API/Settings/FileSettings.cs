namespace ClincManagement.API.Settings
{
    public static class FileSettings
    {
        public partial class ImageSettings
        {
            public const int MaxSizeInMB = 15;
            public const int MaxSizeInBytes = MaxSizeInMB * 1024 * 1024; // بالبايت
            public static readonly string[] AllowedSignatures = new string[]
            {
                "89-50",   // PNG
                "FF-D8",   // JPG / JPEG
                "47-49",   // GIF
                "42-4D"    // BMP
            };
        }

        public partial class InvoiceSettings
        {
            public const int MaxSizeInMB = 100;
            public const int MaxSizeInBytes = MaxSizeInMB * 1024 * 1024;
            public static readonly string AllowedSignature = "25-50-44-46";  // PDF
        }
    }
}
