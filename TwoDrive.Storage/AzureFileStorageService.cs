using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using TwoDrive.Services.__Services__;

namespace TwoDrive.Storage;

internal class AzureFileStorageService(BlobServiceClient _blobServiceClient) : IFileStorageService
{
    public async Task DeleteBulkAsync(string containerName, IEnumerable<string> blobNames, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);
        ArgumentNullException.ThrowIfNull(blobNames);

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        foreach (var blobName in blobNames)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(blobName);

            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
        }
    }

    public async Task DownloadAsync(string containerName, string blobName, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);

        var blobClient = _blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName);

        await blobClient.DownloadContentAsync(ct);
    }

    public async Task UploadBulkAsync(IEnumerable<FileUploadDto> files, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(files);

        var containerClients = new Dictionary<string, BlobContainerClient>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in files)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(file.ContainerName);
            ArgumentException.ThrowIfNullOrWhiteSpace(file.BlobName);
            ArgumentNullException.ThrowIfNull(file.Content);
            ArgumentException.ThrowIfNullOrWhiteSpace(file.ContentType);

            if (!containerClients.TryGetValue(file.ContainerName, out var containerClient))
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(file.ContainerName);
                await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);
                containerClients[file.ContainerName] = containerClient;
            }

            var blobClient = containerClient.GetBlobClient(file.BlobName);
            await blobClient.UploadAsync(
                file.Content,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType },
                    ProgressHandler = new Progress<long>(bytesUploaded =>
                    {
                        // Optionally, report progress here (e.g., log or update a progress bar)
                    })
                },
                ct);
        }
    }
}
