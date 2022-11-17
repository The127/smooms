using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace smooms.api.Models;

public abstract record EntityIdBase
{
    public Guid Value { get; protected internal init; } = Guid.NewGuid();
}

public abstract record EntityIdBase<TId> : EntityIdBase
    where TId : EntityIdBase<TId>, new()
{
    public static TId Parse(string guid)
    {
        return new TId { Value = Guid.Parse(guid) };
    }
    
    public static bool TryParse(string guid, [NotNullWhen(true)] out TId? value)
    {
        value = null;
        
        if (!Guid.TryParse(guid, out var result))
        {
            return false;
        }
        
        value = new TId
        {
            Value = result
        };
        
        return true;
    }
    
    public static TId FromGuid(Guid id)
    {
        return new TId { Value = id };
    }
}

public class EntityIdConverter<TId> : ValueConverter<TId, Guid>
    where TId : EntityIdBase, new()
{
    public EntityIdConverter()
        : base(id => id.Value,
            guid => new TId
            {
                Value = guid,
            })
    {
    }
}

public class EntityIdJsonConverter<TId> : JsonConverter<TId>
    where TId : EntityIdBase, new()
{
    public override TId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var guid = reader.GetGuid();
        return new TId
        {
            Value = guid,
        };
    }

    public override void Write(Utf8JsonWriter writer, TId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}

public static class EntityIdJsonConverterExtensions
{
    public static void AddEntityIdJsonConverterFromAssembly(this JsonSerializerOptions options, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(EntityIdBase)));
        
        foreach (var type in types)
        {
            var converterType = typeof(EntityIdJsonConverter<>).MakeGenericType(type);
            var converter = (JsonConverter) Activator.CreateInstance(converterType)!;
            options.Converters.Add(converter);
        }
    }
    
    public static void AddEntityIdJsonSchemaDefinitionFromAssembly(this SchemaGeneratorOptions schemaGeneratorOptions, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(EntityIdBase)));
        
        foreach (var type in types)
        {
            schemaGeneratorOptions.CustomTypeMappings.Add(type, () => new OpenApiSchema
            {
                Type = "string",
                Format = "guid",
            });
        }
    }
}