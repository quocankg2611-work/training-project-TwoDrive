using TwoDrive.Services.__Persistence__;
using TwoDrive.Services.Common;

namespace TwoDrive.Services.Files;

public class UpdateFileCommand : ICommand
{
    public Guid FileId { get; set; }
    public string? NewName { get; set; }
}

internal class UpdateFileCommandHandler (IFilesRepository filesRepository): ICommandHandler<UpdateFileCommand>
{
    public async Task Handle(UpdateFileCommand command)
    {
        var file = await filesRepository.GetByIdAsync(command.FileId);
        if (file != null && command.NewName != null)
        {
            file.Name = command.NewName;
            await filesRepository.UpdateAsync(file);
        }
    }
}
