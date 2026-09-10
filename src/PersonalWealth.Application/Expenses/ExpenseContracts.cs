using PersonalWealth.Application.Results;
using PersonalWealth.Domain.Expenses;

namespace PersonalWealth.Application.Expenses;

public sealed record ExpenseCategoryModel(Guid Id, Guid TenantId, string Name, Guid? ParentCategoryId, bool IsActive);
public sealed record ExpenseModel(Guid Id, Guid TenantId, Guid CategoryId, DateTime ExpenseDate, decimal Amount, string Description, Guid? BankTransactionId);
public sealed record CreateExpenseCategoryRequest(Guid TenantId, Guid Id, string Name, Guid? ParentCategoryId = null);
public sealed record CreateExpenseRequest(Guid TenantId, Guid Id, Guid CategoryId, DateTime ExpenseDate, decimal Amount, string Description);
public sealed record UpdateExpenseRequest(Guid TenantId, Guid ExpenseId, Guid CategoryId, DateTime ExpenseDate, decimal Amount, string Description);
public sealed record RecurringExpenseModel(Guid Id, Guid TenantId, Guid CategoryId, DateTime StartDate, decimal Amount, string Description, RecurrenceFrequency Frequency, int Interval, DateTime? EndDate, bool IsActive);
public sealed record CreateRecurringExpenseRequest(Guid TenantId, Guid Id, Guid CategoryId, DateTime StartDate, decimal Amount, string Description, RecurrenceFrequency Frequency, int Interval = 1, DateTime? EndDate = null);
public sealed record ExpenseCategoryTotal(Guid CategoryId, decimal TotalAmount, int ExpenseCount);
public sealed record ExpenseMonthlyTotal(DateTime Month, decimal TotalAmount, int ExpenseCount);
public sealed record ExpenseTrendPoint(DateTime Month, decimal TotalAmount);
public sealed record ExpenseReportModel(decimal TotalAmount, int ExpenseCount, IReadOnlyCollection<ExpenseCategoryTotal> ByCategory, IReadOnlyCollection<ExpenseMonthlyTotal> ByMonth, IReadOnlyCollection<ExpenseTrendPoint> Trend);
public sealed record LinkExpenseTransactionRequest(Guid TenantId, Guid ExpenseId, Guid BankTransactionId);

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
    Task LinkBankTransactionAsync(Guid tenantId, Guid expenseId, Guid bankTransactionId, CancellationToken cancellationToken = default);
}

public interface IRecurringExpenseRepository
{
    Task AddAsync(RecurringExpenseModel recurringExpense, CancellationToken cancellationToken = default);
    Task<RecurringExpenseModel?> GetAsync(Guid tenantId, Guid recurringExpenseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RecurringExpenseModel>> ListAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

public interface IExpenseReportingRepository
{
    Task<ExpenseReportModel> GetReportAsync(Guid tenantId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
}

public interface IExpenseService
{
    Task<Result<ExpenseCategoryModel>> CreateCategoryAsync(CreateExpenseCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<ExpenseCategoryModel>>> ListCategoriesAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Result<ExpenseModel>> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<Result<ExpenseModel>> UpdateExpenseAsync(UpdateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<Result<ExpenseModel>> GetExpenseAsync(Guid tenantId, Guid expenseId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<ExpenseModel>>> ListExpensesAsync(Guid tenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    Task<Result<RecurringExpenseModel>> CreateRecurringExpenseAsync(CreateRecurringExpenseRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<DateTime>>> GetRecurringOccurrencesAsync(Guid tenantId, Guid recurringExpenseId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<Result<ExpenseModel>> LinkBankTransactionAsync(LinkExpenseTransactionRequest request, CancellationToken cancellationToken = default);
    Task<Result<ExpenseReportModel>> GetReportAsync(Guid tenantId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
