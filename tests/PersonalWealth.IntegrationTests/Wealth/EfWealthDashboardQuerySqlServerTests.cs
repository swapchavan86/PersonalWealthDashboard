using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PersonalWealth.Infrastructure.Persistence;
using Xunit;

namespace PersonalWealth.IntegrationTests.Wealth;

public sealed class EfWealthDashboardQuerySqlServerTests
{
    [Fact]
    public async Task GenerateCreateScript_ProducesCurrentSchema()
    {
        var sourceConnectionString = Environment.GetEnvironmentVariable("PW_TEST_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(sourceConnectionString))
        {
            throw new InvalidOperationException("PW_TEST_CONNECTION_STRING is required for the SQL Server integration test.");
        }

        var builder = new SqlConnectionStringBuilder(sourceConnectionString)
        {
            InitialCatalog = $"PersonalWealth_DashboardQuery_{Guid.NewGuid():N}"
        };
        var databaseName = builder.InitialCatalog;
        var escapedDatabaseName = databaseName.Replace("]", "]]", StringComparison.Ordinal);

        try
        {
            var options = new DbContextOptionsBuilder<PersonalWealthDbContext>()
                .UseSqlServer(builder.ConnectionString)
                .Options;

            await using var db = new PersonalWealthDbContext(options, new TenantContext(Guid.NewGuid()));
            var script = db.Database.GenerateCreateScript();

            Assert.Contains("CREATE TABLE [BankTransactions]", script, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("CREATE TABLE [InvestmentHoldings]", script, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("CREATE TABLE [AssetValuations]", script, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("CREATE TABLE [Liabilities]", script, StringComparison.OrdinalIgnoreCase);

            await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "generated-schema.sql"), script);
        }
        finally
        {
            var masterBuilder = new SqlConnectionStringBuilder(sourceConnectionString)
            {
                InitialCatalog = "master"
            };

            await using var connection = new SqlConnection(masterBuilder.ConnectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = $"IF DB_ID(N'{databaseName.Replace("'", "''", StringComparison.Ordinal)}') IS NOT NULL BEGIN ALTER DATABASE [{escapedDatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{escapedDatabaseName}]; END";
            await command.ExecuteNonQueryAsync();
        }
    }
}
