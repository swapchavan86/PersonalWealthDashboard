namespace PersonalWealth.Application.Imports;

public sealed record WealthImportResult(
    Guid ImportId,
    int RowsRead,
    int RowsImported,
    IReadOnlyList<string> Errors,
    bool Success);

public interface IWealthWorkbookImportService
{
    Task<WealthImportResult> ImportAsync(Stream content, string fileName, CancellationToken cancellationToken = default);
}
