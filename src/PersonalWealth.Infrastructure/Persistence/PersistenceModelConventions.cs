using Microsoft.EntityFrameworkCore;

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
            if (entityType.FindPrimaryKey() is not null)
            {
                continue;
            }

            var idProperty = entityType.FindProperty("Id");
            if (idProperty is not null)
            {
                entityType.SetPrimaryKey(idProperty);
            }
        }
    }
}
