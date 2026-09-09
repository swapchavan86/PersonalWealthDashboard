using Microsoft.EntityFrameworkCore;
using PersonalWealth.Infrastructure.Persistence;
using Xunit;

namespace PersonalWealth.IntegrationTests.Persistence;

public sealed class EfCoreConventionsTests
{
    [Fact]
    public void Central_conventions_define_key_precision_and_timestamp_mapping()
    {
        var options = new DbContextOptionsBuilder<ConventionTestDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;")
            .Options;

        using var context = new ConventionTestDbContext(options);
        var entityType = context.Model.FindEntityType(typeof(TestPersistenceEntity));

        Assert.NotNull(entityType);
        Assert.Equal(
            nameof(TestPersistenceEntity.Id),
            entityType.FindPrimaryKey()!.Properties.Single().Name);

        Assert.Equal(18, entityType.FindProperty(nameof(TestPersistenceEntity.Amount))!.GetPrecision());
        Assert.Equal(2, entityType.FindProperty(nameof(TestPersistenceEntity.Amount))!.GetScale());
        Assert.Equal(
            "datetime2(7)",
            entityType.FindProperty(nameof(TestPersistenceEntity.CreatedAt))!.GetColumnType());
        Assert.Equal(
            "datetime2(7)",
            entityType.FindProperty(nameof(TestPersistenceEntity.UpdatedAt))!.GetColumnType());
    }

    private sealed class ConventionTestDbContext(DbContextOptions<ConventionTestDbContext> options)
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

    private sealed class TestPersistenceEntity
    {
        public Guid Id { get; init; }

        public decimal Amount { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }
    }
}
