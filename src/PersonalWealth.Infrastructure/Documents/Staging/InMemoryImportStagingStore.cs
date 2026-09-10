using System.Collections.Concurrent;
using PersonalWealth.Application.Documents.Staging;

namespace PersonalWealth.Infrastructure.Documents.Staging;

public sealed class InMemoryImportStagingStore : IImportStagingStore
{
    private readonly ConcurrentDictionary<Guid, ImportRecord> records = new();

    public Task<ImportRecord> CreateAsync(Guid documentId, int stagedRowCount, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var now = DateTimeOffset.UtcNow;
        var record = new ImportRecord(Guid.NewGuid(), documentId, ImportStatus.Staged, stagedRowCount, now, now);
        records[record.ImportId] = record;
        return Task.FromResult(record);
    }

    public Task SaveAsync(ImportRecord record, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        records[record.ImportId] = record;
        return Task.CompletedTask;
    }
}
