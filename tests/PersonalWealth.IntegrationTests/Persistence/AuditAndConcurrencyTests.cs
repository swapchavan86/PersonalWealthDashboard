using Microsoft.EntityFrameworkCore;
using PersonalWealth.Domain.Entities;
using PersonalWealth.Infrastructure.Persistence;
using Xunit;

namespace PersonalWealth.IntegrationTests.Persistence;

public sealed class AuditAndConcurrencyTests
{
    [Fact]
    public void Concurrency_tracked_entity_uses_sql_server_rowversion()
    {
        var options = new DbContextOptionsBuilder<AuditTestDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;")
            .Options;

        using var context = new AuditTestDbContext(options);
        var entityType = context.Model.FindEntityType(typeof(TestPersistenceEntity));
        var property = entityType!.FindProperty(nameof(TestPersistenceEntity.RowVersion));

        Assert.NotNull(property);
        Assert.True(property.IsConcurrencyToken);
        Assert.True(property.ValueGenerated == Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate);
        Assert.Equal("rowversion", property.GetColumnType());
    }

    [Fact]
    public void Audit_metadata_sets_created_timestamp_for_new_entities()
    {
        var options = new DbContextOptionsBuilder<AuditTestDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;")
            .Options;

        using var context = new AuditTestDbContext(options);
        var entity = new TestPersistenceEntity();
        var expected = new DateTime(2026, 9, 10, 4, 30, 0, DateTimeKind.Utc);

        context.Add(entity);
        PersistenceAuditMetadata.Apply(context.ChangeTracker, expected);

        var entry = context.Entry(entity);
        Assert.Equal(expected, entry.Property(nameof(TestPersistenceEntity.CreatedAt)).CurrentValue);
        Assert.Null(entry.Property(nameof(TestPersistenceEntity.UpdatedAt)).CurrentValue);
    }

    [Fact]
    public void Audit_metadata_updates_updated_timestamp_without_modifying_created_timestamp()
    {
        var options = new DbContextOptionsBuilder<AuditTestDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;")
            .Options;

        using var context = new AuditTestDbContext(options);
        var entity = new TestPersistenceEntity();
        var created = new DateTime(2026, 9, 9, 4, 30, 0, DateTimeKind.Utc);
        var updated = new DateTime(2026, 9, 10, 4, 30, 0, DateTimeKind.Utc);

        context.Add(entity);
        PersistenceAuditMetadata.Apply(context.ChangeTracker, created);
        context.Entry(entity).State = EntityState.Modified;
        PersistenceAuditMetadata.Apply(context.ChangeTracker, updated);

        var entry = context.Entry(entity);
        Assert.Equal(created, entry.Property(nameof(TestPersistenceEntity.CreatedAt)).CurrentValue);
        Assert.Equal(updated, entry.Property(nameof(TestPersistenceEntity.UpdatedAt)).CurrentValue);
        Assert.False(entry.Property(nameof(TestPersistenceEntity.CreatedAt)).IsModified);
    }

    private sealed class AuditTestDbContext(DbContextOptions<AuditTestDbContext> options)
        : DbContext(options)
    {
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            PersistenceModelConventions.Configure(configurationBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestPersistenceEntity>();
            PersistenceModelConventions.Apply(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }

    private sealed class TestPersistenceEntity : Entity<Guid>, IAuditableEntity, IConcurrencyTracked
    {
        public TestPersistenceEntity()
            : base(Guid.NewGuid())
        {
        }

        public DateTime CreatedAt { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public byte[] RowVersion { get; private set; } = [];
    }
}
