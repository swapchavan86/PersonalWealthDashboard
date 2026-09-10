using Microsoft.EntityFrameworkCore;
using PersonalWealth.Domain.Entities;

namespace PersonalWealth.Infrastructure.Persistence;

public static class PersistenceModelConventions
{
    public static void Configure(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        configurationBuilder.Properties<DateTime>().HaveColumnType("datetime2(7)");
        configurationBuilder.Properties<DateTime?>().HaveColumnType("datetime2(7)");
    }

    public static void Apply(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.FindPrimaryKey() is null)
            {
                var idProperty = entityType.FindProperty("Id");
                if (idProperty is not null)
                {
                    entityType.SetPrimaryKey(idProperty);
                }
            }

            if (!typeof(IConcurrencyTracked).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var rowVersionProperty = entityType.FindProperty(nameof(IConcurrencyTracked.RowVersion));
            if (rowVersionProperty is null || rowVersionProperty.ClrType != typeof(byte[]))
            {
                throw new InvalidOperationException(
                    $"Concurrency-tracked entity '{entityType.ClrType.Name}' must define a byte[] RowVersion property.");
            }

            modelBuilder.Entity(entityType.ClrType)
                .Property(nameof(IConcurrencyTracked.RowVersion))
                .IsRowVersion();
        }
    }
}
