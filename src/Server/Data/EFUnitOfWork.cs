using System.Diagnostics;
using System.Threading;
using Blazor.HelloGalaxy.Server.Models;

namespace Blazor.HelloGalaxy.Server.Data;

public class EFUnitOfWork : IUnitOfWork
{

    public EFUnitOfWork(ApplicationDbContext context, ILoggerFactory? loggerFactory = null)
    {
        Context = context;
        LoggerFactory = loggerFactory;
    }

    protected internal ApplicationDbContext Context { get; }
    protected internal ILoggerFactory? LoggerFactory { get; }

    public IRepository<T> Repository<T>() where T : class, IEntity =>
        new EFRepository<T>(Context, LoggerFactory?.CreateLogger<EFRepository<T>>());

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!Context.ChangeTracker.HasChanges())
        {
            return;
        }
        await Context.SaveChangesAsync(cancellationToken);
        Context.ChangeTracker.Clear();
    }

    public void SaveChanges()
    {
        if (!Context.ChangeTracker.HasChanges())
        {
            return;
        }
        Context.SaveChanges();
        Context.ChangeTracker.Clear();
    }

    [DebuggerStepThrough]
    public void Dispose()
    {

        GC.SuppressFinalize(this);
    }
}

