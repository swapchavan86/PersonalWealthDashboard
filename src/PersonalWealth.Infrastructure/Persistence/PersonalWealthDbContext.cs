using Microsoft.EntityFrameworkCore;

namespace PersonalWealth.Infrastructure.Persistence;

public sealed class PersonalWealthDbContext(DbContextOptions<PersonalWealthDbContext> options)
    : DbContext(options)
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        PersistenceModelConventions.Configure(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonalWealthDbContext).Assembly);
        PersistenceModelConventions.Apply(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
