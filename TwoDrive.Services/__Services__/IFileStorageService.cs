namespace TwoDrive.Services.__Services__;

public sealed record FileUploadDto(
    string ContainerName,
    string BlobName,
    Stream Content,
    string ContentType,
    Action<long>? Progress
);

public interface IFileStorageService
{
    Task UploadBulkAsync(IEnumerable<FileUploadDto> files, CancellationToken ct = default);
    Task DownloadAsync(string containerName, string blobName, CancellationToken ct = default);
    Task DeleteBulkAsync(string containerName, IEnumerable<string> blobNames, CancellationToken ct = default);
}
