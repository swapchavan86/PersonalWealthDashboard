using Microsoft.EntityFrameworkCore;
using PersonalWealth.Application.Banking;
using PersonalWealth.Application.Events;
using PersonalWealth.Domain.Banking;
using PersonalWealth.Infrastructure.Persistence;
using PersonalWealth.Infrastructure.Persistence.Outbox;

namespace PersonalWealth.Infrastructure.Banking;

public sealed class EfBankImportService(
    PersonalWealthDbContext dbContext,
    ITransactionNormalizer normalizer,
    ITransactionCategorizer categorizer,
    IStatementReconciler reconciler,
    IOutbox outbox) : IBankImportService
{
    public async Task<BankImportResult> ImportAsync(BankImportRequest request, CancellationToken cancellationToken = default)
    {
        if (request.TenantId == Guid.Empty || request.AccountId == Guid.Empty || request.ImportId == Guid.Empty)
            return new(request.ImportId, 0, 0, false, ["Import identifiers must not be empty."]);

        var account = await dbContext.BankAccounts.SingleOrDefaultAsync(x => x.TenantId == request.TenantId && x.Id == request.AccountId, cancellationToken);
        if (account is null) return new(request.ImportId, 0, 0, false, ["Bank account was not found for the active tenant."]);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            if (await dbContext.ImportIdentities.AnyAsync(x => x.TenantId == request.TenantId && x.ContentHash == request.ContentHash, cancellationToken))
            {
                dbContext.ImportDuplicateDecisions.Add(new ImportDuplicateDecision(Guid.NewGuid(), request.TenantId, request.ImportId, "Document", request.ContentHash, true, "Content hash already imported."));
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return new(request.ImportId, 0, request.Transactions.Count, true, []);
            }

            var imported = new List<BankTransaction>();
            var duplicates = 0;
            foreach (var input in request.Transactions)
            {
                var normalized = normalizer.Normalize(input with { TenantId = request.TenantId, AccountId = request.AccountId, ImportId = request.ImportId });
                var category = categorizer.Categorize(normalized);
                normalized = normalized with { Category = category };
                var duplicate = await dbContext.BankTransactions.AnyAsync(x => x.TenantId == request.TenantId && x.AccountId == request.AccountId && x.SourceFingerprint == normalized.SourceFingerprint, cancellationToken);
                dbContext.ImportDuplicateDecisions.Add(new ImportDuplicateDecision(Guid.NewGuid(), request.TenantId, request.ImportId, "Row", normalized.SourceFingerprint, duplicate, duplicate ? "Row fingerprint already imported." : "No existing row fingerprint."));
                if (duplicate) { duplicates++; continue; }
                imported.Add(new BankTransaction(normalized.Id == Guid.Empty ? Guid.NewGuid() : normalized.Id, request.TenantId, request.AccountId, normalized.TransactionDate, normalized.Amount, Enum.Parse<BankTransactionDirection>(normalized.Direction, true), normalized.Description, request.ImportId, normalized.SourceFingerprint));
                imported[^1].SetCategory(category);
            }

            dbContext.BankTransactions.AddRange(imported);
            dbContext.ImportIdentities.Add(new ImportIdentity(Guid.NewGuid(), request.TenantId, request.ImportId, request.ContentHash));
            outbox.Add(new BankImportCommittedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, request.TenantId, request.ImportId, imported.Count));
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var all = request.Transactions;
            var reconciliation = reconciler.Reconcile(request.OpeningBalance ?? 0m, request.ClosingBalance ?? 0m, all);
            return new(request.ImportId, imported.Count, duplicates, reconciliation.IsMatch, reconciliation.IsMatch ? [] : [$"Statement reconciliation difference: {reconciliation.Difference:F2}"]);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
