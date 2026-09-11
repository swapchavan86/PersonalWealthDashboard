using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PersonalWealth.Application.Wealth;
using PersonalWealth.Infrastructure.Persistence;
using PersonalWealth.Infrastructure.Wealth;
using Xunit;

namespace PersonalWealth.IntegrationTests.Wealth;

public sealed class EfWealthDashboardQuerySqlServerTests
{
    [Fact]
    public async Task EmptyTenant_ReturnsZeroDashboard()
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

        try
        {
            var options = new DbContextOptionsBuilder<PersonalWealthDbContext>()
                .UseSqlServer(builder.ConnectionString)
                .Options;

            await using var db = new PersonalWealthDbContext(options, new TenantContext(Guid.NewGuid()));
            await db.Database.MigrateAsync();

            var query = new EfWealthDashboardQuery(db);
            var result = await query.GetAsync();

            Assert.Equal(0m, result.Cash);
            Assert.Equal(0m, result.Investments);
            Assert.Equal(0m, result.OtherAssets);
            Assert.Equal(0m, result.Liabilities);
            Assert.Equal(0m, result.NetWorth);
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
            command.CommandText = $"IF DB_ID(N'{builder.InitialCatalog.Replace("'", "''")}') IS NOT NULL BEGIN ALTER DATABASE [{builder.InitialCatalog.Replace("]", "]]'")}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{builder.InitialCatalog.Replace("]", "]]'")}]; END";
            await command.ExecuteNonQueryAsync();
        }
    }
}
