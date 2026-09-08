namespace PersonalWealth.Domain.Entities;

public interface IEntity<out TId>
{
    TId Id { get; }
}

public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>
{
    protected Entity(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        Id = id;
    }

    public TId Id { get; }

    public bool Equals(Entity<TId>? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public override bool Equals(object? obj) =>
        obj is Entity<TId> other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(GetType(), Id);
}
