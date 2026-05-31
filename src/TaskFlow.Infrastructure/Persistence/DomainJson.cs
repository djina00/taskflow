using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using TaskFlow.SharedKernel.Domain;

namespace TaskFlow.Infrastructure.Persistence;

/// <summary>
/// Shared <see cref="JsonSerializerOptions"/> for persisting domain aggregates.
/// The domain models are deliberately encapsulated — private setters, private
/// constructors and read-only collections backed by private fields — so a stock
/// serializer cannot round-trip them. A <see cref="DefaultJsonTypeInfoResolver"/>
/// modifier teaches System.Text.Json how to rehydrate them without forcing the
/// domain to leak its internals or carry serializer attributes. Both the JSON and
/// SQLite document stores reuse these options so the two technologies persist an
/// identical on-disk shape.
/// </summary>
internal static class DomainJson
{
    private const BindingFlags InstanceMembers =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static JsonSerializerOptions Options { get; } = Build();

    private static JsonSerializerOptions Build()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { ConfigureDomainEntity }
            }
        };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    /// <summary>
    /// Reshapes the contract for every <see cref="BaseEntity"/>: construct through
    /// the non-public parameterless constructor, enable the non-public property
    /// setters, persist collections via their private backing fields, and drop
    /// purely computed getters.
    /// </summary>
    private static void ConfigureDomainEntity(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object) return;

        var type = typeInfo.Type;
        if (!typeof(BaseEntity).IsAssignableFrom(type)) return;

        // Build via the (non-public) parameterless constructor so field
        // initializers run and the backing collections are instantiated.
        var constructor = type.GetConstructor(InstanceMembers, binder: null, Type.EmptyTypes, modifiers: null);
        if (constructor is not null)
            typeInfo.CreateObject = () => constructor.Invoke(null);

        var toRemove = new List<JsonPropertyInfo>();
        var toAdd = new List<JsonPropertyInfo>();

        foreach (var jsonProperty in typeInfo.Properties)
        {
            var clrProperty = type.GetProperty(jsonProperty.Name, InstanceMembers);
            if (clrProperty is null) continue;

            // Honour [JsonIgnore] — notably AggregateRoot.DomainEvents, which is
            // transient state that must never be persisted.
            if (clrProperty.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
            {
                toRemove.Add(jsonProperty);
                continue;
            }

            if (IsReadOnlyCollection(clrProperty.PropertyType, out var elementType))
            {
                // Persist the mutable private backing field rather than the
                // read-only wrapper so the items survive a round-trip.
                var field = FindBackingField(type, clrProperty.Name, elementType);
                if (field is not null)
                {
                    var listType = typeof(List<>).MakeGenericType(elementType);
                    var replacement = typeInfo.CreateJsonPropertyInfo(listType, jsonProperty.Name);
                    replacement.Get = field.GetValue;
                    // No setter: add the items into the list the constructor created
                    // rather than replacing it (the field is read-only).
                    replacement.ObjectCreationHandling = JsonObjectCreationHandling.Populate;
                    toRemove.Add(jsonProperty);
                    toAdd.Add(replacement);
                }
                continue;
            }

            if (clrProperty.SetMethod is null)
            {
                // Purely computed (e.g. IsArchived, IsCompleted) — not persisted.
                toRemove.Add(jsonProperty);
                continue;
            }

            if (jsonProperty.Set is null)
            {
                // Public getter, non-public setter — allow writing on deserialize.
                var setter = clrProperty.SetMethod;
                jsonProperty.Set = (target, value) => setter.Invoke(target, new[] { value });
            }
        }

        foreach (var property in toRemove) typeInfo.Properties.Remove(property);
        foreach (var property in toAdd) typeInfo.Properties.Add(property);
    }

    private static bool IsReadOnlyCollection(Type type, out Type elementType)
    {
        elementType = typeof(object);
        if (type == typeof(string) || !type.IsGenericType) return false;

        var definition = type.GetGenericTypeDefinition();
        if (definition == typeof(IReadOnlyCollection<>) || definition == typeof(IReadOnlyList<>))
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }
        return false;
    }

    private static FieldInfo? FindBackingField(Type type, string propertyName, Type elementType)
    {
        var listType = typeof(List<>).MakeGenericType(elementType);

        for (var current = type; current is not null && current != typeof(object); current = current.BaseType)
        {
            foreach (var field in current.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (field.FieldType == listType &&
                    string.Equals(field.Name.TrimStart('_'), propertyName, StringComparison.OrdinalIgnoreCase))
                    return field;
            }
        }
        return null;
    }
}
