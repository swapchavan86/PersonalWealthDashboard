using Microsoft.EntityFrameworkCore;
using PersonalWealth.Application.Tenancy;
using PersonalWealth.Domain.Entities;
using PersonalWealth.Infrastructure.Persistence.Outbox;
using PersonalWealth.Infrastructure.Persistence.ProcessedEvents;

namespace PersonalWealth.Infrastructure.Persistence;

public class PersonalWealthDbContext(
    DbContextOptions<PersonalWealthDbContext> options,
    ITenantContext tenantContext)
    : DbContext(options)
{
    private readonly ITenantContext tenantContext = tenantContext
        ?? throw new ArgumentNullException(nameof(tenantContext));

    internal Guid CurrentTenantId => tenantContext.TenantId;

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        PersistenceModelConventions.Configure(configurationBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess = true)
    {
        TenantPersistenceEnforcement.Validate(ChangeTracker, tenantContext);
        PersistenceAuditMetadata.Apply(ChangeTracker, DateTime.UtcNow);
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        TenantPersistenceEnforcement.Validate(ChangeTracker, tenantContext);
        PersistenceAuditMetadata.Apply(ChangeTracker, DateTime.UtcNow);
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonalWealthDbContext).Assembly);
        PersistenceModelConventions.Apply(modelBuilder);
        TenantPersistenceEnforcement.ApplyQueryFilters(modelBuilder, this);
        base.OnModelCreating(modelBuilder);
    }
}
