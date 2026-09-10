using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonalWealth.Infrastructure.Persistence.Outbox;

namespace PersonalWealth.Worker;

public sealed class OutboxPublisherWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<OutboxPublisherOptions> options,
    ILogger<OutboxPublisherWorker> logger)
    : BackgroundService
{
    private readonly OutboxPublisherOptions options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            try
            {
                var publisher = scope.ServiceProvider.GetRequiredService<OutboxPublisher>();
                var processed = await publisher.PublishPendingAsync(stoppingToken);
                if (processed > 0)
                {
                    logger.LogInformation("Outbox publisher processed {Count} messages.", processed);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Outbox publisher iteration failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(options.PollIntervalSeconds), stoppingToken);
        }
    }
}
