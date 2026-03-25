namespace TwoDrive.Persistence.Models
{
    public class FolderPersistence
    {
        public Guid Id { get; set; }
        public Guid? ParentFolderId { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public FolderPersistence? ParentFolder { get; set; }
        public ICollection<FolderPersistence> ChildFolders { get; set; } = new List<FolderPersistence>();
        public ICollection<FilePersistence> Files { get; set; } = new List<FilePersistence>();
    }
}
