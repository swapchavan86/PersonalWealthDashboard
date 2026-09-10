using PersonalWealth.Application.Results;

namespace PersonalWealth.Application.Expenses;

public sealed record ExpenseCategoryModel(Guid Id, Guid TenantId, string Name, Guid? ParentCategoryId, bool IsActive);

public sealed record ExpenseModel(Guid Id, Guid TenantId, Guid CategoryId, DateTime ExpenseDate, decimal Amount, string Description, Guid? BankTransactionId);

public sealed record CreateExpenseCategoryRequest(Guid TenantId, Guid Id, string Name, Guid? ParentCategoryId = null);

public sealed record CreateExpenseRequest(Guid TenantId, Guid Id, Guid CategoryId, DateTime ExpenseDate, decimal Amount, string Description);

public sealed record UpdateExpenseRequest(Guid TenantId, Guid ExpenseId, Guid CategoryId, DateTime ExpenseDate, decimal Amount, string Description);

public interface IExpenseCategoryRepository
{
    Task AddAsync(ExpenseCategoryModel category, CancellationToken cancellationToken = default);
    Task<ExpenseCategoryModel?> GetAsync(Guid tenantId, Guid categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ExpenseCategoryModel>> ListAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

public interface IExpenseRepository
{
    Task AddAsync(ExpenseModel expense, CancellationToken cancellationToken = default);
    Task<ExpenseModel?> GetAsync(Guid tenantId, Guid expenseId, CancellationToken cancellationToken = default);
    Task UpdateAsync(ExpenseModel expense, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ExpenseModel>> ListAsync(Guid tenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
}

public interface IExpenseService
{
    Task<Result<ExpenseCategoryModel>> CreateCategoryAsync(CreateExpenseCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<ExpenseCategoryModel>>> ListCategoriesAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Result<ExpenseModel>> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<Result<ExpenseModel>> UpdateExpenseAsync(UpdateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<Result<ExpenseModel>> GetExpenseAsync(Guid tenantId, Guid expenseId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<ExpenseModel>>> ListExpensesAsync(Guid tenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
}
