namespace TaskFlow.SharedKernel.Domain;

/// <summary>
/// Base class for every domain entity. Provides a strongly-typed identity
/// and identity-based equality. Lives in the SharedKernel so it carries no
/// dependency on persistence, UI, or any specific module.
/// </summary>
public abstract class BaseEntity : IEquatable<BaseEntity>
{
    protected BaseEntity(Guid id) => Id = id;

    /// <summary>Parameterless constructor for serializers / ORMs.</summary>
    protected BaseEntity() => Id = Guid.NewGuid();

    public Guid Id { get; protected set; }

    public bool Equals(BaseEntity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return GetType() == other.GetType() && Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) => Equals(obj as BaseEntity);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(BaseEntity? left, BaseEntity? right) => Equals(left, right);

    public static bool operator !=(BaseEntity? left, BaseEntity? right) => !Equals(left, right);
}
