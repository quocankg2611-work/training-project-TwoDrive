using TwoDrive.Services.Files;

namespace TwoDrive.Services.__Persistence__;

public interface IFilesRepository
{
    Task<FileDetailsDto?> GetByIdAsync(Guid fileId);
    Task DeleteAsync(Guid fileId);
    Task<UploadFileCommandResult> UploadAsync(UploadFileCommand command);
    Task UploadFilesAsync(UploadFilesCommand command);
}
