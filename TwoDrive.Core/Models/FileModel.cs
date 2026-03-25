namespace TwoDrive.Core.Models
{
    public class FileModel
    {
        public Guid Id { get; set; }
        public Guid? FolderId { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public string StorageKey { get; set; } = string.Empty;
        public string Checksum { get; set; } = string.Empty;
    }
}
