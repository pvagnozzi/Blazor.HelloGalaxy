using Blazor.HelloGalaxy.Server.Models;

namespace Blazor.HelloGalaxy.Server.Services;

public interface IUserService : IDisposable
{
    Task<User?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

    Task<User?> RegisterAsync(User user, CancellationToken cancellationToken = default);
}

