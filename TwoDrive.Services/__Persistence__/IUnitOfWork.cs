namespace TwoDrive.Services.__Persistence__;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
