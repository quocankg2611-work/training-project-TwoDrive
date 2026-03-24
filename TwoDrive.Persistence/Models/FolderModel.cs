namespace TwoDrive.Persistence.Models
{
    public class FolderModel
    {
        public Guid Id { get; set; }
        public Guid? ParentFolderId { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public FolderModel? ParentFolder { get; set; }
        public ICollection<FolderModel> ChildFolders { get; set; } = new List<FolderModel>();
        public ICollection<FileModel> Files { get; set; } = new List<FileModel>();
    }
}
