using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TwoDrive.IntegrationTest.TestInfrastructure;

namespace TwoDrive.IntegrationTest;

[Collection(TestContainersCollection.Name)]
public sealed class UploadFilesEndpointTests(TestContainersFixture testContainersFixture)
{
    [Fact]
    public async Task UploadFiles_ShouldStoreMetadata_AndReturnUploadedFileInDocumentsQuery()
    {
        await using var factory = new TwoDriveApiFactory(testContainersFixture);
        await factory.InitializeDatabaseAsync();

        using var client = factory.CreateClient();

        var folderSuffix = Guid.NewGuid().ToString("N");
        var basePath = $"root-{folderSuffix}";
        const string filePath = "finance";
        const string fileName = "report.txt";

        using var uploadContent = new MultipartFormDataContent
        {
            { new StringContent(basePath), "BasePath" },
            { new StringContent(filePath), "FilePaths" }
        };

        var fileBytes = Encoding.UTF8.GetBytes("hello from integration test");
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        uploadContent.Add(fileContent, "Files", fileName);

        var uploadResponse = await client.PostAsync("/files/upload", uploadContent);

        Assert.Equal(HttpStatusCode.OK, uploadResponse.StatusCode);

        var queryResponse = await client.GetAsync($"/documents?Path={basePath}/{filePath}");

        Assert.Equal(HttpStatusCode.OK, queryResponse.StatusCode);

        await using var stream = await queryResponse.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);

        var items = document.RootElement.GetProperty("items");
        var names = items.EnumerateArray().Select(item => item.GetProperty("name").GetString()).ToList();

        Assert.Contains("report", names);
    }
}
