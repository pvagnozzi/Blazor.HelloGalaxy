using System.Linq.Expressions;
using Blazor.HelloGalaxy.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor.HelloGalaxy.Server.Data;

public class EFRepository<T> : IRepository<T> where T : class, IEntity
{
    public EFRepository(DbContext context, ILogger? logger = null)
    {
        Context = context;
        Logger = logger;
        DbSet = Context.Set<T>();
    }

    protected internal DbContext Context { get; }
    protected internal DbSet<T> DbSet { get; }
    protected internal ILogger? Logger { get; }

    public virtual IList<T> List(Expression<Func<T, bool>>? filterExpression = null)
    {
        Logger?.LogInformation("List {type}: {expression}", typeof(T).FullName, filterExpression);
        var result = filterExpression is null ? DbSet : DbSet.Where(filterExpression);
        return result.ToList();
    }

    public virtual T? First(Expression<Func<T, bool>> filterExpression)
    {
        Logger?.LogInformation("First {type}: {expression}", typeof(T).FullName, filterExpression);
        return DbSet.FirstOrDefault(filterExpression);
    }

    public virtual T? GetById(string id)
    {
        Logger?.LogInformation("{name}: GetById: {id}", GetRepositoryName(), id);
        return DbSet.FirstOrDefault(x => x.Id == id);
    }

    public virtual void DeleteById(string id)
    {
        Logger?.LogInformation("{name}: DeleteByIdAsync {id}", GetRepositoryName(), id);
        var item = GetById(id);
        if (item is null)
        {
            throw new InvalidOperationException($"{typeof(T).FullName} '{id}' does not exists");
        }
        DbSet.Remove(item);
    }

    public virtual string Insert(T item)
    {
        Logger?.LogInformation("{name}: Insert {item}", GetRepositoryName(), item);

        if (item is not Entity entity)
        {
            throw new InvalidOperationException($"{typeof(T)} is an invalid entity");
        }

        entity.CreatedOn = DateTime.UtcNow;
        entity.UpdatedOn = DateTime.UtcNow;
        DbSet.Add(item);
        return item.Id;
    }

    public virtual void Update(T item)
    {
        Logger?.LogInformation("{name}: Update {item}", GetRepositoryName(), item);
        if (item is not Entity entity)
        {
            throw new InvalidOperationException($"{typeof(T)} is an invalid entity");
        }

        entity.UpdatedOn = DateTime.UtcNow;
        Attach(item);
        DbSet.Update(item);
    }

    public virtual void InsertRange(IEnumerable<T> items)
    {
        Logger?.LogInformation("{name}: InsertRange {items}", GetRepositoryName(), items);
        DbSet.AddRange(items);
    }

    public async Task<IList<T>> ListAsync(Expression<Func<T, bool>>? filterExpression = null,
        CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("ListAsync {type}: {expression}", typeof(T).FullName, filterExpression);
        var result = filterExpression is null ? DbSet : DbSet.Where(filterExpression);
        return await result.ToListAsync(cancellationToken);
    }

    public Task<T?> FirstAsync(Expression<Func<T, bool>> filterExpression, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("FirstAsync {type}: {expression}", typeof(T).FullName, filterExpression);
        return DbSet.FirstOrDefaultAsync(filterExpression, cancellationToken);
    }

    public virtual Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("{name}: GetByIdAsync: {id}", GetRepositoryName(), id);
        return DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task DeleteAsync(T item, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("{name}: DeleteAsync {id}", GetRepositoryName(), item.Id);
        DbSet.Remove(item);
        return Task.CompletedTask;
    }

    public virtual async Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("{name}: DeleteByIdAsync {id}", GetRepositoryName(), id);
        var item = await DbSet.FirstAsync(x => x.Id == id, cancellationToken);
        DbSet.Remove(item);
    }

    public virtual async Task<string> InsertAsync(T item, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("{name}: InsertAsync {item}", GetRepositoryName(), item);

        if (item is not Entity entity)
        {
            throw new InvalidOperationException($"{typeof(T)} is an invalid entity");
        }

        entity.CreatedOn = DateTime.Now;
        entity.UpdatedOn = DateTime.UtcNow;
        await DbSet.AddAsync(item, cancellationToken);
        return item.Id;
    }

    public virtual Task UpdateAsync(T item, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("{name}: UpdateAsync {item}", GetRepositoryName(), item);

        if (item is not Entity entity)
        {
            throw new InvalidOperationException($"{typeof(T)} is an invalid entity");
        }
        entity.UpdatedOn = DateTime.UtcNow;

        Attach(item);
        DbSet.Update(item);
        return Task.CompletedTask;
    }

    public virtual Task InsertRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("{name}: InsertRangeAsync {items}", GetRepositoryName(), items);
        return DbSet.AddRangeAsync(items, cancellationToken);
    }

    public Task DeleteRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("{name}: InsertRangeAsync {items}", GetRepositoryName(), items);
        var data = items.ToArray();
        foreach (var item in data)
        {
            Attach(item);
        }
        DbSet.RemoveRange(data);
        return Task.CompletedTask;
    }

    public Task DeleteByIdRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        Logger?.LogInformation("{name}: DeleteByIdRangeAsync {items}", GetRepositoryName(), ids);
        var toDelete = DbSet.Where(x => ids.Any(i => x.Id == i));
        return DeleteRangeAsync(toDelete, cancellationToken);
    }

    public void Dispose()
    {
        Logger?.LogInformation("{name}: Disposed", GetRepositoryName());
        GC.SuppressFinalize(this);
    }

    protected virtual string GetRepositoryName() => $"EFRepository({typeof(T).FullName})";

    protected virtual void Attach(T item)
    {
        var entry = DbSet.Entry(item);
        if (entry.State == EntityState.Detached)
        {
            DbSet.Attach(item);
        }
    }
}

