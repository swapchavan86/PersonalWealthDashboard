namespace PersonalWealth.Application.Banking;

public sealed record BankAccountModel(
    Guid Id,
    Guid TenantId,
    string Institution,
    string AccountNumber,
    string AccountType,
    string Currency,
    string Status);

public sealed record BankTransactionModel(
    Guid Id,
    Guid TenantId,
    Guid AccountId,
    DateTime TransactionDate,
    decimal Amount,
    string Direction,
    string Description,
    Guid ImportId,
    string SourceFingerprint,
    string? Category,
    Guid? TransferId);

public interface IBankAccountRepository
{
    Task AddAsync(BankAccountModel account, CancellationToken cancellationToken = default);
    Task<BankAccountModel?> GetAsync(Guid tenantId, Guid accountId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<BankAccountModel>> ListAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

public interface IBankTransactionRepository
{
    Task<bool> ExistsByFingerprintAsync(Guid tenantId, Guid accountId, string fingerprint, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IReadOnlyCollection<BankTransactionModel> transactions, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<BankTransactionModel>> ListAsync(Guid tenantId, Guid accountId, CancellationToken cancellationToken = default);
}

public interface IBankImportService
{
    Task<BankImportResult> ImportAsync(BankImportRequest request, CancellationToken cancellationToken = default);
}

public sealed record BankImportRequest(
    Guid TenantId,
    Guid AccountId,
    Guid ImportId,
    string ContentHash,
    IReadOnlyCollection<BankTransactionModel> Transactions,
    decimal? OpeningBalance = null,
    decimal? ClosingBalance = null);

public sealed record BankImportResult(
    Guid ImportId,
    int ImportedCount,
    int DuplicateCount,
    bool Reconciled,
    IReadOnlyCollection<string> Errors);
