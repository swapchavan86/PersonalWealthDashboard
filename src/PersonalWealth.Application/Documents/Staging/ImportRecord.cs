namespace PersonalWealth.Application.Documents.Staging;

public sealed record ImportRecord(
    Guid ImportId,
    Guid DocumentId,
    ImportStatus Status,
    int StagedRowCount,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    string? Error = null)
{
    public ImportRecord MoveTo(ImportStatus status, DateTimeOffset atUtc, string? error = null) =>
        this with { Status = status, UpdatedAtUtc = atUtc, Error = error };
}
