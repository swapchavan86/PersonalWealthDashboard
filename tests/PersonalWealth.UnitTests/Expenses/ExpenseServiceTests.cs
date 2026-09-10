using PersonalWealth.Application.Expenses;
using Xunit;

namespace PersonalWealth.UnitTests.Expenses;

public sealed class ExpenseServiceTests
{
    [Fact]
    public async Task CreateCategory_requires_existing_parent()
    {
        var tenantId = Guid.NewGuid();
        var categories = new FakeCategoryRepository();
        var service = new ExpenseService(categories, new FakeExpenseRepository());

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
        var service = new ExpenseService(categories, new FakeExpenseRepository());

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
        var service = new ExpenseService(categories, expenses);

        var created = await service.CreateExpenseAsync(new CreateExpenseRequest(tenantId, expenseId, categoryId, new DateTime(2026, 9, 10, 18, 30, 0), 250m, "  Dinner  "));
        var updated = await service.UpdateExpenseAsync(new UpdateExpenseRequest(tenantId, expenseId, categoryId, new DateTime(2026, 9, 11, 8, 0, 0), 300m, "  Dinner updated  "));

        Assert.True(created.IsSuccess);
        Assert.True(updated.IsSuccess);
        Assert.Equal(tenantId, updated.Value.TenantId);
        Assert.Equal(new DateTime(2026, 9, 11), updated.Value.ExpenseDate);
        Assert.Equal("Dinner updated", updated.Value.Description);
        Assert.Equal(300m, updated.Value.Amount);
    }

    private sealed class FakeCategoryRepository : IExpenseCategoryRepository
    {
        private readonly Dictionary<(Guid TenantId, Guid Id), ExpenseCategoryModel> items = new();

        public Task AddAsync(ExpenseCategoryModel category, CancellationToken cancellationToken = default)
        {
            items[(category.TenantId, category.Id)] = category;
            return Task.CompletedTask;
        }

        public Task<ExpenseCategoryModel?> GetAsync(Guid tenantId, Guid categoryId, CancellationToken cancellationToken = default) =>
            Task.FromResult(items.GetValueOrDefault((tenantId, categoryId)));

        public Task<IReadOnlyCollection<ExpenseCategoryModel>> ListAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<ExpenseCategoryModel>>(items.Values.Where(x => x.TenantId == tenantId).ToArray());
    }

    private sealed class FakeExpenseRepository : IExpenseRepository
    {
        private readonly Dictionary<(Guid TenantId, Guid Id), ExpenseModel> items = new();

        public Task AddAsync(ExpenseModel expense, CancellationToken cancellationToken = default)
        {
            items[(expense.TenantId, expense.Id)] = expense;
            return Task.CompletedTask;
        }

        public Task<ExpenseModel?> GetAsync(Guid tenantId, Guid expenseId, CancellationToken cancellationToken = default) =>
            Task.FromResult(items.GetValueOrDefault((tenantId, expenseId)));

        public Task UpdateAsync(ExpenseModel expense, CancellationToken cancellationToken = default)
        {
            items[(expense.TenantId, expense.Id)] = expense;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<ExpenseModel>> ListAsync(Guid tenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
        {
            var query = items.Values.Where(x => x.TenantId == tenantId);
            if (from.HasValue) query = query.Where(x => x.ExpenseDate >= from.Value.Date);
            if (to.HasValue) query = query.Where(x => x.ExpenseDate <= to.Value.Date);
            return Task.FromResult<IReadOnlyCollection<ExpenseModel>>(query.OrderBy(x => x.ExpenseDate).ToArray());
        }
    }
}
