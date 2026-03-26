namespace TwoDrive.Core.Models;

public class FolderModel
{
    public Guid Id { get; set; }
    public Guid? ParentFolderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;

    public string PathForChildren => $"{Path.TrimEnd('/')}/{Name}";
}
