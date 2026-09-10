using PersonalWealth.Application.Expenses;
using PersonalWealth.Domain.Expenses;
using Xunit;

namespace PersonalWealth.UnitTests.Expenses;

public sealed class ExpensePhaseTests
{
    [Fact]
    public async Task Recurring_rule_occurrences_are_exposed_by_application_service()
    {
        var tenantId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var recurringId = Guid.NewGuid();
        var recurring = new RecurringExpenseModel(recurringId, tenantId, categoryId, new DateTime(2026, 1, 10), 100m, "Subscription", RecurrenceFrequency.Monthly, 1, null, true);
        var service = new ExpenseService(new CategoryRepository(categoryId, tenantId), new ExpenseRepository(), new RecurringRepository(recurring), new ReportingRepository());

        var result = await service.GetRecurringOccurrencesAsync(tenantId, recurringId, new DateTime(2026, 2, 1), new DateTime(2026, 4, 30));

        Assert.True(result.IsSuccess);
        Assert.Equal(new[] { new DateTime(2026, 2, 10), new DateTime(2026, 3, 10), new DateTime(2026, 4, 10) }, result.Value);
    }

    [Fact]
    public async Task Report_query_returns_application_projection()
    {
        var tenantId = Guid.NewGuid();
        var expected = new ExpenseReportModel(1500m, 2,
            new[] { new ExpenseCategoryTotal(Guid.NewGuid(), 1500m, 2) },
            new[] { new ExpenseMonthlyTotal(new DateTime(2026, 9, 1), 1500m, 2) },
            new[] { new ExpenseTrendPoint(new DateTime(2026, 9, 1), 1500m) });
        var service = new ExpenseService(new CategoryRepository(Guid.NewGuid(), tenantId), new ExpenseRepository(), new RecurringRepository(), new ReportingRepository(expected));

        var result = await service.GetReportAsync(tenantId, new DateTime(2026, 9, 1), new DateTime(2026, 9, 30));

        Assert.True(result.IsSuccess);
        Assert.Equal(1500m, result.Value.TotalAmount);
        Assert.Equal(2, result.Value.ExpenseCount);
    }

    [Fact]
    public async Task Bank_transaction_linking_updates_canonical_expense_projection()
    {
        var tenantId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var expenseId = Guid.NewGuid();
        var repository = new ExpenseRepository();
        await repository.AddAsync(new ExpenseModel(expenseId, tenantId, categoryId, new DateTime(2026, 9, 10), 500m, "Fuel", null));
        var service = new ExpenseService(new CategoryRepository(categoryId, tenantId), repository, new RecurringRepository(), new ReportingRepository());

        var result = await service.LinkBankTransactionAsync(new LinkExpenseTransactionRequest(tenantId, expenseId, Guid.NewGuid()));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.BankTransactionId);
    }

    private sealed class CategoryRepository(Guid categoryId, Guid tenantId) : IExpenseCategoryRepository
    {
        public Task AddAsync(ExpenseCategoryModel category, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<ExpenseCategoryModel?> GetAsync(Guid tenantIdValue, Guid categoryIdValue, CancellationToken cancellationToken = default) => Task.FromResult<ExpenseCategoryModel?>(tenantIdValue == tenantId && categoryIdValue == categoryId ? new ExpenseCategoryModel(categoryId, tenantId, "General", null, true) : null);
        public Task<IReadOnlyCollection<ExpenseCategoryModel>> ListAsync(Guid tenantIdValue, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<ExpenseCategoryModel>>(Array.Empty<ExpenseCategoryModel>());
    }

    private sealed class ExpenseRepository : IExpenseRepository
    {
        private readonly Dictionary<(Guid, Guid), ExpenseModel> items = new();
        public Task AddAsync(ExpenseModel expense, CancellationToken cancellationToken = default) { items[(expense.TenantId, expense.Id)] = expense; return Task.CompletedTask; }
        public Task<ExpenseModel?> GetAsync(Guid tenantId, Guid expenseId, CancellationToken cancellationToken = default) => Task.FromResult(items.GetValueOrDefault((tenantId, expenseId)));
        public Task UpdateAsync(ExpenseModel expense, CancellationToken cancellationToken = default) { items[(expense.TenantId, expense.Id)] = expense; return Task.CompletedTask; }
        public Task<IReadOnlyCollection<ExpenseModel>> ListAsync(Guid tenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<ExpenseModel>>(items.Values.Where(x => x.TenantId == tenantId).ToArray());
        public Task LinkBankTransactionAsync(Guid tenantId, Guid expenseId, Guid bankTransactionId, CancellationToken cancellationToken = default) { var item = items[(tenantId, expenseId)]; items[(tenantId, expenseId)] = item with { BankTransactionId = bankTransactionId }; return Task.CompletedTask; }
    }

    private sealed class RecurringRepository(RecurringExpenseModel? item = null) : IRecurringExpenseRepository
    {
        public Task AddAsync(RecurringExpenseModel recurringExpense, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<RecurringExpenseModel?> GetAsync(Guid tenantId, Guid recurringExpenseId, CancellationToken cancellationToken = default) => Task.FromResult(item is not null && item.TenantId == tenantId && item.Id == recurringExpenseId ? item : null);
        public Task<IReadOnlyCollection<RecurringExpenseModel>> ListAsync(Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<RecurringExpenseModel>>(Array.Empty<RecurringExpenseModel>());
    }

    private sealed class ReportingRepository(ExpenseReportModel? report = null) : IExpenseReportingRepository
    {
        public Task<ExpenseReportModel> GetReportAsync(Guid tenantId, DateTime from, DateTime to, CancellationToken cancellationToken = default) => Task.FromResult(report ?? new ExpenseReportModel(0m, 0, Array.Empty<ExpenseCategoryTotal>(), Array.Empty<ExpenseMonthlyTotal>(), Array.Empty<ExpenseTrendPoint>()));
    }
}
