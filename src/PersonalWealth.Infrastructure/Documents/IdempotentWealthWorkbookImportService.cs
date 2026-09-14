using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PersonalWealth.Application.Imports;
using PersonalWealth.Application.Tenancy;
using PersonalWealth.Domain.Banking;
using PersonalWealth.Infrastructure.Persistence;

namespace PersonalWealth.Infrastructure.Documents;

public sealed class IdempotentWealthWorkbookImportService(
    WealthWorkbookImportService inner,
    PersonalWealthDbContext db,
    ITenantContext tenantContext) : IWealthWorkbookImportService
{
    private static readonly string[] CsvHeaders = ["RecordType","ExternalId","Date","Institution","AccountNumber","AccountType","Currency","Description","Direction","Amount","Category","SecuritySymbol","SecurityName","SecurityType","Quantity","UnitPrice","Fees","AssetName","AssetType","AcquisitionValue","LiabilityName","LiabilityType","Principal","InterestRate","MaturityDate","PrincipalAmount","InterestAmount","ValuationValue"];

    public async Task<WealthImportResult> ImportAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        await using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();
        var hash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        if (await db.ImportIdentities.AnyAsync(x => x.ContentHash == hash, cancellationToken))
            return new(Guid.Empty, 0, 0, [], true);

        Stream importContent = buffer;
        var importFileName = fileName;
        if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            buffer.Position = 0;
            ExcelWorkbookReadResult workbook;
            try { workbook = WealthExcelWorkbookReader.Read(buffer, cancellationToken); }
            catch (Exception ex) { return new(Guid.Empty, 0, 0, [$"Excel workbook could not be read: {ex.Message}"], false); }
            if (workbook.Errors.Count > 0)
                return new(Guid.Empty, workbook.Rows.Count, 0, workbook.Errors, false);
            if (workbook.Rows.Count == 0)
                return new(Guid.Empty, 0, 0, ["The workbook contains no data rows."], false);

            var csv = new StringBuilder();
            csv.AppendLine(string.Join(',', CsvHeaders));
            foreach (var row in workbook.Rows)
            {
                cancellationToken.ThrowIfCancellationRequested();
                csv.AppendLine(string.Join(',', CsvHeaders.Select(header => Csv(row.GetValueOrDefault(header, string.Empty)))));
            }
            importContent = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));
            importFileName = Path.ChangeExtension(fileName, ".csv");
        }
        else if (!fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return new(Guid.Empty, 0, 0, ["Unsupported file type. Upload the PersonalWealth Excel template (.xlsx) or the legacy CSV template (.csv)."], false);
        }

        importContent.Position = 0;
        var result = await inner.ImportAsync(importContent, importFileName, cancellationToken);
        if (result.Success)
        {
            db.ImportIdentities.Add(new ImportIdentity(Guid.NewGuid(), tenantContext.TenantId, result.ImportId, hash));
            await db.SaveChangesAsync(cancellationToken);
        }
        return result;
    }

    private static string Csv(string value) => $"\"{value.Replace("\"", "\"\"")}\"";
}
