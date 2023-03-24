using Blazor.HelloGalaxy.Server.Models;

namespace Blazor.HelloGalaxy.Server.Data;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class, IEntity;
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    void SaveChanges();
}

