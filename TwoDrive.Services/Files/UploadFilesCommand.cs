using TwoDrive.Services.Common;

namespace TwoDrive.Services.Files;

public class UploadFilesCommand : ICommand
{
    public class Item
    {
        public string Path { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public Stream FileStream { get; set; } = null!;
    }

    public string BasePath { get; set; } = string.Empty;
    public List<Item> FilesData { get; set; } = [];
}
