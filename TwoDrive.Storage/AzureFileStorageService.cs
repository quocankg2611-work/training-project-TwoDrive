using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Concurrent;
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

        var filesToUpload = files.ToList();

        foreach (var file in filesToUpload)
        {
            ArgumentNullException.ThrowIfNull(file);
            ArgumentException.ThrowIfNullOrWhiteSpace(file.ContainerName);
            ArgumentException.ThrowIfNullOrWhiteSpace(file.BlobName);
            ArgumentNullException.ThrowIfNull(file.Content);
            ArgumentException.ThrowIfNullOrWhiteSpace(file.ContentType);
        }

        var containerClients = new ConcurrentDictionary<string, BlobContainerClient>(StringComparer.OrdinalIgnoreCase);

        var ensureContainerTasks = filesToUpload
            .Select(file => file.ContainerName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(async containerName =>
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);
                containerClients[containerName] = containerClient;
            });

        await Task.WhenAll(ensureContainerTasks);

        var uploadTasks = filesToUpload.Select(async file =>
        {
            var containerClient = containerClients[file.ContainerName];
            var blobClient = containerClient.GetBlobClient(file.BlobName);
            await blobClient.UploadAsync(
                file.Content,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType },
                    ProgressHandler = new Progress<long>(bytesUploaded =>
                    {
                        // Optionally, report progress here (e.g., log or update a progress bar)
                    }),
                },
                ct);
        });

        await Task.WhenAll(uploadTasks);
    }
}
