using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Services.Files;

public class UploadFileCommand : ICommand
{
    public Guid FolderId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public Stream FileStream { get; set; } = null!;
}

public record UploadFileCommandResult(Guid FileId, string StorageKey);

internal class UploadFileCommandHandler : ICommandHandler<UploadFileCommand, UploadFileCommandResult>
{
    private readonly IFilesRepository _filesRepository;

    public UploadFileCommandHandler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public Task<UploadFileCommandResult> Handle(UploadFileCommand command)
    {
        return _filesRepository.UploadAsync(command);
    }
}
