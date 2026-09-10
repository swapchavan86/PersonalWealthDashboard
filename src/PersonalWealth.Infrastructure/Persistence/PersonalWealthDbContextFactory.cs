using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PersonalWealth.Infrastructure.Persistence;

public sealed class PersonalWealthDbContextFactory
    : IDesignTimeDbContextFactory<PersonalWealthDbContext>
{
    public PersonalWealthDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        var connectionString = configuration.GetConnectionString(PersistenceOptions.ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{PersistenceOptions.ConnectionStringName}' is required for design-time DbContext creation.");
        }

        var options = new DbContextOptionsBuilder<PersonalWealthDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new PersonalWealthDbContext(options, new TenantContext(Guid.NewGuid()));
    }

    private static IConfiguration BuildConfiguration()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var candidateDirectories = new[]
        {
            currentDirectory,
            Path.Combine(currentDirectory, "src", "PersonalWealth.Api"),
            Path.Combine(currentDirectory, "..", "PersonalWealth.Api"),
            Path.Combine(currentDirectory, "..", "..", "PersonalWealth.Api")
        };

        var apiDirectory = candidateDirectories
            .Select(Path.GetFullPath)
            .FirstOrDefault(directory =>
                File.Exists(Path.Combine(directory, "appsettings.json")) ||
                File.Exists(Path.Combine(directory, "appsettings.Local.json")))
            ?? currentDirectory;

        return new ConfigurationBuilder()
            .SetBasePath(apiDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
