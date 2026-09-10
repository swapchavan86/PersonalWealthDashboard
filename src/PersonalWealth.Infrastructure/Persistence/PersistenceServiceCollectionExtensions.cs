using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PersonalWealth.Application.Events;
using PersonalWealth.Infrastructure.Persistence.Outbox;

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

        services.AddOptions<PersistenceOptions>()
            .Configure(options => options.ConnectionString = connectionString)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString));

        services.AddDbContext<PersonalWealthDbContext>((serviceProvider, options) =>
        {
            var persistenceOptions = serviceProvider
                .GetRequiredService<IOptions<PersistenceOptions>>()
                .Value;

            options.UseSqlServer(persistenceOptions.ConnectionString);
        });

        services.AddScoped<IOutbox, EfCoreOutbox>();

        return services;
    }
}
