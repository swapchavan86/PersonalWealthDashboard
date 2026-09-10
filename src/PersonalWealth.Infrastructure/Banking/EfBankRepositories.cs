using Microsoft.EntityFrameworkCore;
using PersonalWealth.Application.Banking;
using PersonalWealth.Domain.Banking;
using PersonalWealth.Infrastructure.Persistence;

namespace PersonalWealth.Infrastructure.Banking;

public sealed class EfBankAccountRepository(PersonalWealthDbContext dbContext) : IBankAccountRepository
{
    public async Task AddAsync(BankAccountModel account, CancellationToken cancellationToken = default)
    {
        var entity = new BankAccount(account.Id, account.TenantId, account.Institution, account.AccountNumber, Enum.Parse<BankAccountType>(account.AccountType, true), account.Currency);
        dbContext.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<BankAccountModel?> GetAsync(Guid tenantId, Guid accountId, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Set<BankAccount>().SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == accountId, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyCollection<BankAccountModel>> ListAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await dbContext.Set<BankAccount>().Where(x => x.TenantId == tenantId).Select(x => new BankAccountModel(x.Id, x.TenantId, x.Institution, x.AccountNumber, x.AccountType.ToString(), x.Currency, x.Status.ToString())).ToListAsync(cancellationToken);

    private static BankAccountModel Map(BankAccount x) => new(x.Id, x.TenantId, x.Institution, x.AccountNumber, x.AccountType.ToString(), x.Currency, x.Status.ToString());
}

public sealed class EfBankTransactionRepository(PersonalWealthDbContext dbContext) : IBankTransactionRepository
{
    public Task<bool> ExistsByFingerprintAsync(Guid tenantId, Guid accountId, string fingerprint, CancellationToken cancellationToken = default) =>
        dbContext.Set<BankTransaction>().AnyAsync(x => x.TenantId == tenantId && x.AccountId == accountId && x.SourceFingerprint == fingerprint, cancellationToken);

    public async Task AddRangeAsync(IReadOnlyCollection<BankTransactionModel> transactions, CancellationToken cancellationToken = default)
    {
        var entities = transactions.Select(x => new BankTransaction(x.Id, x.TenantId, x.AccountId, x.TransactionDate, x.Amount, Enum.Parse<BankTransactionDirection>(x.Direction, true), x.Description, x.ImportId, x.SourceFingerprint)).ToList();
        foreach (var pair in transactions.Zip(entities)) pair.Second.SetCategory(pair.First.Category);
        dbContext.AddRange(entities);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<BankTransactionModel>> ListAsync(Guid tenantId, Guid accountId, CancellationToken cancellationToken = default) =>
        await dbContext.Set<BankTransaction>().Where(x => x.TenantId == tenantId && x.AccountId == accountId).OrderBy(x => x.TransactionDate).Select(x => new BankTransactionModel(x.Id, x.TenantId, x.AccountId, x.TransactionDate, x.Amount, x.Direction.ToString(), x.Description, x.ImportId, x.SourceFingerprint, x.Category, x.TransferId)).ToListAsync(cancellationToken);
}
