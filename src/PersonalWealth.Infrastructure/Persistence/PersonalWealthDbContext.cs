using Microsoft.EntityFrameworkCore;

namespace PersonalWealth.Infrastructure.Persistence;

public sealed class PersonalWealthDbContext(DbContextOptions<PersonalWealthDbContext> options)
    : DbContext(options)
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        PersistenceModelConventions.Configure(configurationBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess = true)
    {
        PersistenceAuditMetadata.Apply(ChangeTracker, DateTime.UtcNow);
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        PersistenceAuditMetadata.Apply(ChangeTracker, DateTime.UtcNow);
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonalWealthDbContext).Assembly);
        PersistenceModelConventions.Apply(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
