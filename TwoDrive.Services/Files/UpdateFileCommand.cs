using TwoDrive.Services.Common;

namespace TwoDrive.Services.Files;

public class UpdateFileCommand : ICommand
{
    public Guid FileId { get; set; }
    public string? NewName { get; set; }
}

internal class UpdateFileCommandHandler : ICommandHandler<UpdateFileCommand>
{
    public Task Handle(UpdateFileCommand command)
    {
        throw new NotImplementedException();
    }
}
