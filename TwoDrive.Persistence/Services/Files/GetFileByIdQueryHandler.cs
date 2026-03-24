using Microsoft.EntityFrameworkCore;
using TwoDrive.Services.Common;
using TwoDrive.Services.Files;

namespace TwoDrive.Persistence.Services.Files;

internal class GetFileByIdQueryHandler : IQueryHandler<GetFileByIdQuery, FileDetailsDto>
{
    private readonly AppDbContext _dbContext;

    public GetFileByIdQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FileDetailsDto> Handle(GetFileByIdQuery query)
    {
        var file = await _dbContext.Files
            .AsNoTracking()
            .Where(x => x.Id == query.FileId)
            .Select(x => new FileDetailsDto(
                x.Id,
                x.FolderId,
                x.Name,
                x.MimeType,
                x.SizeBytes,
                x.StorageKey,
                x.Checksum,
                x.CreatedAt,
                x.UpdatedAt))
            .SingleOrDefaultAsync();

        return file ?? throw new KeyNotFoundException($"File '{query.FileId}' was not found.");
    }
}
