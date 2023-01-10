namespace DocumentsStorage.Core.Entities;

public abstract class Entity
{
    public virtual long Id { get; set; }

    protected Entity()
    {
    }

    protected Entity(long id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetUnproxiedType(this) != GetUnproxiedType(other))
            return false;

        if (Id.Equals(default) || other.Id.Equals(default))
            return false;

        return Id.Equals(other.Id);
    }

    public static bool operator ==(Entity? a, Entity? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return (GetUnproxiedType(this)?.ToString() + Id).GetHashCode();
    }

    internal static Type? GetUnproxiedType(object obj)
    {
        const string EfCoreProxyPrefix = "Castle.Proxies.";

        var type = obj.GetType();
        var typeString = type.ToString();

        return typeString.Contains(EfCoreProxyPrefix) ? type.BaseType : type;
    }
}