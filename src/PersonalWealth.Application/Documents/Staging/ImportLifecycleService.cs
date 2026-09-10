namespace PersonalWealth.Application.Documents.Staging;

public sealed class ImportLifecycleService(IImportStagingStore store)
{
    public Task<ImportRecord> StageAsync(Guid documentId, int rowCount, CancellationToken cancellationToken = default) =>
        store.CreateAsync(documentId, rowCount, cancellationToken);

    public async Task<ImportRecord> TransitionAsync(
        ImportRecord record,
        ImportStatus nextStatus,
        DateTimeOffset atUtc,
        string? error = null,
        CancellationToken cancellationToken = default)
    {
        if (!IsAllowed(record.Status, nextStatus))
            throw new InvalidOperationException($"Import cannot transition from {record.Status} to {nextStatus}.");

        var updated = record.MoveTo(nextStatus, atUtc, error);
        await store.SaveAsync(updated, cancellationToken);
        return updated;
    }

    private static bool IsAllowed(ImportStatus current, ImportStatus next) => current switch
    {
        ImportStatus.Discovered => next == ImportStatus.Staged || next == ImportStatus.Failed,
        ImportStatus.Staged => next == ImportStatus.Validated || next == ImportStatus.Rejected || next == ImportStatus.Failed,
        ImportStatus.Validated => next == ImportStatus.Committed || next == ImportStatus.Rejected || next == ImportStatus.Failed,
        ImportStatus.Rejected => false,
        ImportStatus.Committed => false,
        ImportStatus.Failed => false,
        _ => false
    };
}
