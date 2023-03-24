using System.Linq.Expressions;
using Blazor.HelloGalaxy.Server.Models;

namespace Blazor.HelloGalaxy.Server.Data;

public interface IRepository<T> : IDisposable
    where T : class, IEntity
{
    IList<T> List(Expression<Func<T, bool>>? filterExpression = null);
    T? First(Expression<Func<T, bool>> filterExpression);
    T? GetById(string id);
    string Insert(T item);
    void Update(T item);
    void DeleteById(string id);
    void InsertRange(IEnumerable<T> items);

    Task<IList<T>> ListAsync(Expression<Func<T, bool>>? filterExpression = null, CancellationToken cancellationToken = default);
    Task<T?> FirstAsync(Expression<Func<T, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<string> InsertAsync(T item, CancellationToken cancellationToken = default);
    Task UpdateAsync(T item, CancellationToken cancellationToken = default);
    Task DeleteAsync(T item, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default);
    Task InsertRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);
    Task DeleteByIdRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
}

