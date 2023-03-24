namespace Blazor.HelloGalaxy.Server.Models;

public class Entity : IEntity
{
    public Entity(string? id = null, DateTimeOffset? createdOn = null, DateTimeOffset? updatedOn = null)
    {
        Id = id ?? Guid.NewGuid().ToString();
        CreatedOn = createdOn ?? DateTimeOffset.UtcNow;
        UpdatedOn = updatedOn ?? DateTimeOffset.UtcNow;
    }

    public string Id { get; protected internal set; }
    public DateTimeOffset CreatedOn { get; protected internal set; }
    public DateTimeOffset UpdatedOn { get; protected internal set; }
}

