using Blazor.HelloGalaxy.Server.Models;

namespace Blazor.HelloGalaxy.Server.Data;

public interface IRepositoryFactory : IDisposable
{
    IRepository<T> CreateRepository<T>() where T : class, IEntity;

    Task InitializeAsync(CancellationToken cancellation = default);
}

