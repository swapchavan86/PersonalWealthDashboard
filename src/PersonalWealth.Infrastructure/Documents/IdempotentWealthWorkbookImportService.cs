using System.Security.Cryptography;
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
    public async Task<WealthImportResult> ImportAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        await using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);
        var hash = Convert.ToHexString(SHA256.HashData(buffer.ToArray())).ToLowerInvariant();
        if (await db.ImportIdentities.AnyAsync(x => x.ContentHash == hash, cancellationToken))
            return new(Guid.Empty, 0, 0, [], true);

        buffer.Position = 0;
        var result = await inner.ImportAsync(buffer, fileName, cancellationToken);
        if (result.Success)
        {
            db.ImportIdentities.Add(new ImportIdentity(Guid.NewGuid(), tenantContext.TenantId, result.ImportId, hash));
            await db.SaveChangesAsync(cancellationToken);
        }
        return result;
    }
}
