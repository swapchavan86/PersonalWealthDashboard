using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PersonalWealth.Infrastructure.Persistence;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(PersistenceOptions.ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{PersistenceOptions.ConnectionStringName}' is required.");
        }

        services.AddDbContext<PersonalWealthDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
