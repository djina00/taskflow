using TaskFlow.SharedKernel.Domain;
using TaskFlow.SharedKernel.Results;

namespace TaskFlow.Modules.Users.Domain;

/// <summary>
/// A named authorization role (Role-Based Access Control). Roles are reference
/// data drawn from a small, well-known catalogue rather than created ad hoc, so
/// the canonical instances are exposed as static members and resolved by name.
/// Identity-based equality is inherited from <see cref="BaseEntity"/>, which is
/// why the catalogue entries carry stable, hard-coded identifiers.
/// </summary>
public sealed class Role : BaseEntity
{
    public static readonly Role Admin = new(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Admin");
    public static readonly Role Manager = new(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Manager");
    public static readonly Role Member = new(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Member");

    private Role(Guid id, string name) : base(id) => Name = name;

    /// <summary>Parameterless constructor for serializers.</summary>
    private Role() { }

    public string Name { get; private set; } = string.Empty;

    /// <summary>The complete role catalogue.</summary>
    public static IReadOnlyCollection<Role> All => new[] { Admin, Manager, Member };

    /// <summary>
    /// Resolves a role from the catalogue by name (case-insensitive), returning
    /// a failed <see cref="Result"/> instead of throwing for an unknown name.
    /// </summary>
    public static Result<Role> FromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("A role name is required.");

        var role = All.FirstOrDefault(r =>
            string.Equals(r.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));

        return role is null
            ? Error.NotFound($"Role '{name}' does not exist.")
            : role;
    }

    public override string ToString() => Name;
}
