using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Persistence.Repositories;

internal class UnitOfWork(AppDbContext appDbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync()
    {
        await appDbContext.SaveChangesAsync();
    }
}
