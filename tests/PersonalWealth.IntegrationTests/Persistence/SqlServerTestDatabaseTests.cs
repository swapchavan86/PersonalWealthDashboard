using Xunit;

namespace PersonalWealth.IntegrationTests.Persistence;

public sealed class SqlServerTestDatabaseTests
{
    [Fact]
    public async Task Test_database_can_initialize_and_reset()
    {
        await using var database = SqlServerTestDatabase.Create();

        await database.ResetAsync();
        await using (var initializedContext = database.CreateContext())
        {
            Assert.True(await initializedContext.Database.CanConnectAsync());
        }

        await database.ResetAsync();
        await using var resetContext = database.CreateContext();
        Assert.True(await resetContext.Database.CanConnectAsync());
    }

    [Fact]
    public void Test_database_requires_an_explicit_test_connection_string()
    {
        var originalValue = Environment.GetEnvironmentVariable("PW_TEST_CONNECTION_STRING");
        Environment.SetEnvironmentVariable("PW_TEST_CONNECTION_STRING", null);

        try
        {
            var exception = Assert.Throws<InvalidOperationException>(SqlServerTestDatabase.Create);

            Assert.Contains("PW_TEST_CONNECTION_STRING", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            Environment.SetEnvironmentVariable("PW_TEST_CONNECTION_STRING", originalValue);
        }
    }

    [Fact]
    public void Test_database_uses_a_unique_database_name_per_fixture()
    {
        const string sourceConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;";
        var originalValue = Environment.GetEnvironmentVariable("PW_TEST_CONNECTION_STRING");
        Environment.SetEnvironmentVariable("PW_TEST_CONNECTION_STRING", sourceConnectionString);

        try
        {
            var first = SqlServerTestDatabase.Create();
            var second = SqlServerTestDatabase.Create();

            Assert.NotEqual(first.ConnectionString, second.ConnectionString);
            Assert.Contains("PersonalWealth_IntegrationTests_", first.ConnectionString, StringComparison.Ordinal);
            Assert.Contains("PersonalWealth_IntegrationTests_", second.ConnectionString, StringComparison.Ordinal);
        }
        finally
        {
            Environment.SetEnvironmentVariable("PW_TEST_CONNECTION_STRING", originalValue);
        }
    }
}
