using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace smooms.app.Models;

public abstract class EntityBase<TId>
    where TId : EntityIdBase<TId>, new()
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local - EF Core needs a setter
    public TId Id { get; private set; } = new();
}

public abstract class EntityBaseConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : EntityBase<TId>
    where TId : EntityIdBase<TId>, new()
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        ApplyTypedIdConverters(builder);
        ApplyEnumConverters(builder);
    }
    
    private void ApplyTypedIdConverters(EntityTypeBuilder<TEntity> builder)
    {
        foreach (var propertyInfo in typeof(TEntity).GetProperties()
                     .Where(x => x.PropertyType.IsAssignableTo(typeof(EntityIdBase))))
        {
            var idConverterType = typeof(EntityIdConverter<>).MakeGenericType(propertyInfo.PropertyType);
            var idConverterConstructor = idConverterType.GetConstructor(Array.Empty<Type>());
            var idConverter = (ValueConverter)idConverterConstructor!.Invoke(Array.Empty<object>());

            builder.Property(propertyInfo.Name)
                .HasConversion(idConverter);
        }
    }

    private void ApplyEnumConverters(EntityTypeBuilder<TEntity> builder)
    {
        foreach (var propertyInfo in typeof(TEntity).GetProperties()
                     .Where(x => x.PropertyType.IsEnum))
        {
            var enumConverterType = typeof(EnumConverter<>).MakeGenericType(propertyInfo.PropertyType);
            var enumConverterConstructor = enumConverterType.GetConstructor(Array.Empty<Type>());
            var enumConverter = (ValueConverter)enumConverterConstructor!.Invoke(Array.Empty<object>());

            builder.Property(propertyInfo.Name)
                .HasConversion(enumConverter);
        }
    }
}

public class EnumConverter<TEnum> : ValueConverter<TEnum, string>
    where TEnum : Enum
{
    public EnumConverter() : base(
        x => x.ToString(),
        x => (TEnum)Enum.Parse(typeof(TEnum), x))
    {
    }
}