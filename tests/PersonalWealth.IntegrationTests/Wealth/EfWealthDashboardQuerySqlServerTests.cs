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
        var connectionString = Environment.GetEnvironmentVariable("PW_TEST_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Assert.Skip("PW_TEST_CONNECTION_STRING is required for the SQL Server integration test.");
        }

        var tenant = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PersonalWealthDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var db = new PersonalWealthDbContext(options, new TenantContext(tenant));
        var query = new EfWealthDashboardQuery(db);

        var result = await query.GetAsync();

        Assert.Equal(0m, result.Cash);
        Assert.Equal(0m, result.Investments);
        Assert.Equal(0m, result.OtherAssets);
        Assert.Equal(0m, result.Liabilities);
        Assert.Equal(0m, result.NetWorth);
    }
}
