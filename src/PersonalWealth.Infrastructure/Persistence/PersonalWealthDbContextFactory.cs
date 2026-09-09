using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PersonalWealth.Infrastructure.Persistence;

public sealed class PersonalWealthDbContextFactory
    : IDesignTimeDbContextFactory<PersonalWealthDbContext>
{
    public PersonalWealthDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString(PersistenceOptions.ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{PersistenceOptions.ConnectionStringName}' is required for design-time DbContext creation.");
        }

        var options = new DbContextOptionsBuilder<PersonalWealthDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new PersonalWealthDbContext(options);
    }
}
