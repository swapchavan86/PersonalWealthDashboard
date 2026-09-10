using Microsoft.EntityFrameworkCore;
using PersonalWealth.Application.Tenancy;
using PersonalWealth.Domain.Assets;
using PersonalWealth.Domain.Banking;
using PersonalWealth.Domain.Entities;
using PersonalWealth.Domain.Expenses;
using PersonalWealth.Domain.Investments;
using PersonalWealth.Domain.Liabilities;
using PersonalWealth.Infrastructure.Persistence.Outbox;
using PersonalWealth.Infrastructure.Persistence.ProcessedEvents;

namespace PersonalWealth.Infrastructure.Persistence;

public class PersonalWealthDbContext(DbContextOptions<PersonalWealthDbContext> options, ITenantContext tenantContext) : DbContext(options)
{
    private readonly ITenantContext tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    internal Guid CurrentTenantId => tenantContext.TenantId;
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();
    public DbSet<ImportIdentity> ImportIdentities => Set<ImportIdentity>();
    public DbSet<ImportDuplicateDecision> ImportDuplicateDecisions => Set<ImportDuplicateDecision>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<RecurringExpense> RecurringExpenses => Set<RecurringExpense>();
    public DbSet<InvestmentAccount> InvestmentAccounts => Set<InvestmentAccount>();
    public DbSet<Security> Securities => Set<Security>();
    public DbSet<InvestmentHolding> InvestmentHoldings => Set<InvestmentHolding>();
    public DbSet<InvestmentTransaction> InvestmentTransactions => Set<InvestmentTransaction>();
    public DbSet<CorporateAction> CorporateActions => Set<CorporateAction>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetValuation> AssetValuations => Set<AssetValuation>();
    public DbSet<Liability> Liabilities => Set<Liability>();
    public DbSet<LiabilityRepayment> LiabilityRepayments => Set<LiabilityRepayment>();
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) => PersistenceModelConventions.Configure(configurationBuilder);
    public override int SaveChanges(bool acceptAllChangesOnSuccess = true) { TenantPersistenceEnforcement.Validate(ChangeTracker, tenantContext); PersistenceAuditMetadata.Apply(ChangeTracker, DateTime.UtcNow); return base.SaveChanges(acceptAllChangesOnSuccess); }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) { TenantPersistenceEnforcement.Validate(ChangeTracker, tenantContext); PersistenceAuditMetadata.Apply(ChangeTracker, DateTime.UtcNow); return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken); }
    protected override void OnModelCreating(ModelBuilder modelBuilder) { modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersonalWealthDbContext).Assembly); PersistenceModelConventions.Apply(modelBuilder); TenantPersistenceEnforcement.ApplyQueryFilters(modelBuilder, this); base.OnModelCreating(modelBuilder); }
}
