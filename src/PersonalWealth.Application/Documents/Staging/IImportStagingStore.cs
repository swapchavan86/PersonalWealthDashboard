namespace PersonalWealth.Application.Documents.Staging;

public interface IImportStagingStore
{
    Task<ImportRecord> CreateAsync(Guid documentId, int stagedRowCount, CancellationToken cancellationToken = default);
    Task SaveAsync(ImportRecord record, CancellationToken cancellationToken = default);
}
