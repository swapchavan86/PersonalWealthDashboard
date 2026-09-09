using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalWealth.Infrastructure.Persistence;
using Xunit;

namespace PersonalWealth.IntegrationTests.Persistence;

public sealed class PersistenceFoundationTests
{
    [Fact]
    public void DbContext_can_be_constructed_with_valid_sql_server_configuration()
    {
        using var context = CreateContext();

        Assert.Equal(
            "Microsoft.EntityFrameworkCore.SqlServer",
            context.Database.ProviderName);
    }

    [Fact]
    public void Model_creation_does_not_require_database_access()
    {
        using var context = CreateContext();

        var model = context.Model;

        Assert.NotNull(model);
    }

    [Fact]
    public void Persistence_registration_adds_the_db_context()
    {
        var configuration = ConfigurationWithConnectionString();
        using var provider = new ServiceCollection()
            .AddPersistence(configuration)
            .BuildServiceProvider();

        using var context = provider.GetRequiredService<PersonalWealthDbContext>();

        Assert.Equal(
            "Microsoft.EntityFrameworkCore.SqlServer",
            context.Database.ProviderName);
    }

    [Fact]
    public void Missing_connection_string_fails_registration_clearly()
    {
        var configuration = new ConfigurationManager();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            new ServiceCollection().AddPersistence(configuration));

        Assert.Contains("Default", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Design_time_factory_reads_environment_configuration()
    {
        const string environmentVariable = "ConnectionStrings__Default";
        var originalValue = Environment.GetEnvironmentVariable(environmentVariable);
        Environment.SetEnvironmentVariable(
            environmentVariable,
            "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;");

        try
        {
            using var context = new PersonalWealthDbContextFactory().CreateDbContext([]);

            Assert.Equal(
                "Microsoft.EntityFrameworkCore.SqlServer",
                context.Database.ProviderName);
        }
        finally
        {
            Environment.SetEnvironmentVariable(environmentVariable, originalValue);
        }
    }

    private static PersonalWealthDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PersonalWealthDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;")
            .Options;

        return new PersonalWealthDbContext(options);
    }

    private static IConfiguration ConfigurationWithConnectionString()
    {
        var configuration = new ConfigurationManager();
        configuration["ConnectionStrings:Default"] =
            "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;";
        return configuration;
    }
}
