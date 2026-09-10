using PersonalWealth.Application.Expenses;
using PersonalWealth.Domain.Expenses;
using Xunit;

namespace PersonalWealth.UnitTests.Expenses;

public sealed class ExpenseServiceTests
{
    [Fact]
    public async Task CreateCategory_requires_existing_parent()
    {
        var tenantId = Guid.NewGuid();
        var categories = new FakeCategoryRepository();
        var service = CreateService(categories, new FakeExpenseRepository());
        var result = await service.CreateCategoryAsync(new CreateExpenseCategoryRequest(tenantId, Guid.NewGuid(), "Food", Guid.NewGuid()));
        Assert.True(result.IsFailure);
        Assert.Equal("not_found", result.Error.Code);
    }

    [Fact]
    public async Task CreateExpense_requires_active_category()
    {
        var tenantId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var categories = new FakeCategoryRepository();
        await categories.AddAsync(new ExpenseCategoryModel(categoryId, tenantId, "Food", null, false));
        var service = CreateService(categories, new FakeExpenseRepository());
        var result = await service.CreateExpenseAsync(new CreateExpenseRequest(tenantId, Guid.NewGuid(), categoryId, new DateTime(2026, 9, 10), 100m, "Lunch"));
        Assert.True(result.IsFailure);
        Assert.Equal("validation", result.Error.Code);
    }

    [Fact]
    public async Task Create_and_update_expense_preserve_tenant_and_normalize_values()
    {
        var tenantId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var expenseId = Guid.NewGuid();
        var categories = new FakeCategoryRepository();
        await categories.AddAsync(new ExpenseCategoryModel(categoryId, tenantId, "Food", null, true));
        var expenses = new FakeExpenseRepository();
        var service = CreateService(categories, expenses);

        var created = await service.CreateExpenseAsync(new CreateExpenseRequest(tenantId, expenseId, categoryId, new DateTime(2026, 9, 10, 18, 30, 0), 250m, "  Dinner  "));
        var updated = await service.UpdateExpenseAsync(new UpdateExpenseRequest(tenantId, expenseId, categoryId, new DateTime(2026, 9, 11, 8, 0, 0), 300m, "  Dinner updated  "));

        Assert.True(created.IsSuccess);
        Assert.True(updated.IsSuccess);
        Assert.Equal(tenantId, updated.Value.TenantId);
        Assert.Equal(new DateTime(2026, 9, 11), updated.Value.ExpenseDate);
        Assert.Equal("Dinner updated", updated.Value.Description);
        Assert.Equal(300m, updated.Value.Amount);
    }

    [Fact]
    public void Recurring_expense_generates_deterministic_monthly_occurrences()
    {
        var rule = new RecurringExpense(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 1, 15), 1000m, "Rent", RecurrenceFrequency.Monthly);
        var occurrences = rule.GetOccurrences(new DateTime(2026, 2, 1), new DateTime(2026, 4, 30));
        Assert.Equal(new[] { new DateTime(2026, 2, 15), new DateTime(2026, 3, 15), new DateTime(2026, 4, 15) }, occurrences);
    }

    [Fact]
    public void Recurring_expense_respects_end_date_and_inactive_state()
    {
        var rule = new RecurringExpense(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2026, 1, 1), 500m, "Subscription", RecurrenceFrequency.Monthly, 1, new DateTime(2026, 1, 31));
        Assert.Single(rule.GetOccurrences(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31)));
        rule.Deactivate();
        Assert.Empty(rule.GetOccurrences(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31)));
    }

    private static ExpenseService CreateService(FakeCategoryRepository categories, FakeExpenseRepository expenses) =>
        new(categories, expenses, new FakeRecurringExpenseRepository(), new FakeReportingRepository());

    private sealed class FakeCategoryRepository : IExpenseCategoryRepository
    {
        private readonly Dictionary<(Guid TenantId, Guid Id), ExpenseCategoryModel> items = new();
        public Task AddAsync(ExpenseCategoryModel category, CancellationToken cancellationToken = default) { items[(category.TenantId, category.Id)] = category; return Task.CompletedTask; }
        public Task<ExpenseCategoryModel?> GetAsync(Guid tenantId, Guid categoryId, CancellationToken cancellationToken = default) => Task.FromResult(items.GetValueOrDefault((tenantId, categoryId)));
        public Task<IReadOnlyCollection<ExpenseCategoryModel>> ListAsync(Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<ExpenseCategoryModel>>(items.Values.Where(x => x.TenantId == tenantId).ToArray());
    }

    private sealed class FakeExpenseRepository : IExpenseRepository
    {
        private readonly Dictionary<(Guid TenantId, Guid Id), ExpenseModel> items = new();
        public Task AddAsync(ExpenseModel expense, CancellationToken cancellationToken = default) { items[(expense.TenantId, expense.Id)] = expense; return Task.CompletedTask; }
        public Task<ExpenseModel?> GetAsync(Guid tenantId, Guid expenseId, CancellationToken cancellationToken = default) => Task.FromResult(items.GetValueOrDefault((tenantId, expenseId)));
        public Task UpdateAsync(ExpenseModel expense, CancellationToken cancellationToken = default) { items[(expense.TenantId, expense.Id)] = expense; return Task.CompletedTask; }
        public Task<IReadOnlyCollection<ExpenseModel>> ListAsync(Guid tenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<ExpenseModel>>(items.Values.Where(x => x.TenantId == tenantId).OrderBy(x => x.ExpenseDate).ToArray());
        public Task LinkBankTransactionAsync(Guid tenantId, Guid expenseId, Guid bankTransactionId, CancellationToken cancellationToken = default) { var item = items[(tenantId, expenseId)]; items[(tenantId, expenseId)] = item with { BankTransactionId = bankTransactionId }; return Task.CompletedTask; }
    }

    private sealed class FakeRecurringExpenseRepository : IRecurringExpenseRepository
    {
        private readonly Dictionary<(Guid TenantId, Guid Id), RecurringExpenseModel> items = new();
        public Task AddAsync(RecurringExpenseModel recurringExpense, CancellationToken cancellationToken = default) { items[(recurringExpense.TenantId, recurringExpense.Id)] = recurringExpense; return Task.CompletedTask; }
        public Task<RecurringExpenseModel?> GetAsync(Guid tenantId, Guid recurringExpenseId, CancellationToken cancellationToken = default) => Task.FromResult(items.GetValueOrDefault((tenantId, recurringExpenseId)));
        public Task<IReadOnlyCollection<RecurringExpenseModel>> ListAsync(Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<RecurringExpenseModel>>(items.Values.Where(x => x.TenantId == tenantId).ToArray());
    }

    private sealed class FakeReportingRepository : IExpenseReportingRepository
    {
        public Task<ExpenseReportModel> GetReportAsync(Guid tenantId, DateTime from, DateTime to, CancellationToken cancellationToken = default) => Task.FromResult(new ExpenseReportModel(0m, 0, Array.Empty<ExpenseCategoryTotal>(), Array.Empty<ExpenseMonthlyTotal>(), Array.Empty<ExpenseTrendPoint>()));
    }
}
