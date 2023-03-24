using System.Security.Cryptography;
using System.Text;
using Blazor.HelloGalaxy.Server.Data;
using Blazor.HelloGalaxy.Server.Models;

namespace Blazor.HelloGalaxy.Server.Services;

public class UserService : IUserService
{
    public UserService(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    private IUnitOfWork UnitOfWork { get; }

    public async Task<User?> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var repository = UnitOfWork.Repository<User>();
        var hash = GetPasswordHash(password);
        var user = await repository.FirstAsync(x => x.UserName == username, cancellationToken);

        if (user is null || user.Password != hash)
        {
            throw new InvalidOperationException("Invalid user or password");
        }

        return user;
    }

    public async Task<User?> RegisterAsync(User user, CancellationToken cancellationToken)
    {
        user.Password = GetPasswordHash(user.Password);
        var repository = UnitOfWork.Repository<User>();
        await repository.InsertAsync(user, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        return await repository.GetByIdAsync(user.Id, cancellationToken);
    }

    private static string GetPasswordHash(string password)
    {
        var data = Encoding.ASCII.GetBytes(password);
        using var hash = SHA256.Create();
        var hashData = hash.ComputeHash(data);
        var sb = new StringBuilder(hashData.Length * 2);
        foreach (var i in hashData)
        {
            sb.Append(i.ToString("X2"));
        }

        return sb.ToString();
    }

    public void Dispose()
    {
        UnitOfWork.Dispose();
        GC.SuppressFinalize(this);
    }
}

