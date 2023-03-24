namespace Blazor.HelloGalaxy.Server.Models;

public interface IEntity
{
    string Id { get; }
    DateTimeOffset CreatedOn { get; }
    DateTimeOffset UpdatedOn { get; }
}

