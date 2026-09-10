namespace PersonalWealth.Infrastructure.Persistence.Outbox;

public sealed class OutboxPublisherOptions
{
    public const string SectionName = "OutboxPublisher";

    public int BatchSize { get; set; } = 50;

    public int PollIntervalSeconds { get; set; } = 5;

    public int MaxAttempts { get; set; } = 5;

    public int RetryDelaySeconds { get; set; } = 30;
}
