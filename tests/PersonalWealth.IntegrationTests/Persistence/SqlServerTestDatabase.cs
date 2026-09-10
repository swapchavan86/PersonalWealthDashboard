using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PersonalWealth.Infrastructure.Persistence;

namespace PersonalWealth.IntegrationTests.Persistence;

public sealed class SqlServerTestDatabase : IAsyncDisposable
{
    private const string TestConnectionStringEnvironmentVariable = "PW_TEST_CONNECTION_STRING";

    private SqlServerTestDatabase(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }

    public static SqlServerTestDatabase Create()
    {
        var sourceConnectionString = Environment.GetEnvironmentVariable(
            TestConnectionStringEnvironmentVariable);

        if (string.IsNullOrWhiteSpace(sourceConnectionString))
        {
            throw new InvalidOperationException(
                $"Environment variable '{TestConnectionStringEnvironmentVariable}' must contain a test-only SQL Server connection string.");
        }

        var builder = new SqlConnectionStringBuilder(sourceConnectionString)
        {
            InitialCatalog = $"PersonalWealth_IntegrationTests_{Guid.NewGuid():N}"
        };

        return new SqlServerTestDatabase(builder.ConnectionString);
    }

    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }

    public PersonalWealthDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PersonalWealthDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new PersonalWealthDbContext(options, new TenantContext(Guid.NewGuid()));
    }

    public async ValueTask DisposeAsync()
    {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
    }
}
