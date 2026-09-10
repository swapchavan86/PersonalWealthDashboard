using PersonalWealth.Application.Results;
using PersonalWealth.Domain.Expenses;

namespace PersonalWealth.Application.Expenses;

public sealed class ExpenseService(
    IExpenseCategoryRepository categoryRepository,
    IExpenseRepository expenseRepository,
    IRecurringExpenseRepository recurringExpenseRepository,
    IExpenseReportingRepository reportingRepository) : IExpenseService
{
    public async Task<Result<ExpenseCategoryModel>> CreateCategoryAsync(CreateExpenseCategoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.TenantId == Guid.Empty) return Failure<ExpenseCategoryModel>("TenantId must not be empty.");
            if (request.Id == Guid.Empty) return Failure<ExpenseCategoryModel>("Category Id must not be empty.");
            if (request.ParentCategoryId.HasValue && await categoryRepository.GetAsync(request.TenantId, request.ParentCategoryId.Value, cancellationToken) is null)
                return Result<ExpenseCategoryModel>.Failure(ApplicationError.NotFound("Parent category was not found."));
            if (await categoryRepository.GetAsync(request.TenantId, request.Id, cancellationToken) is not null)
                return Result<ExpenseCategoryModel>.Failure(ApplicationError.Conflict("Category already exists."));

            var category = new ExpenseCategory(request.Id, request.TenantId, request.Name, request.ParentCategoryId);
            var model = new ExpenseCategoryModel(category.Id, category.TenantId, category.Name, category.ParentCategoryId, category.IsActive);
            await categoryRepository.AddAsync(model, cancellationToken);
            return Result<ExpenseCategoryModel>.Success(model);
        }
        catch (ArgumentException ex) { return Result<ExpenseCategoryModel>.Failure(ApplicationError.Validation(ex.Message)); }
    }

    public async Task<Result<IReadOnlyCollection<ExpenseCategoryModel>>> ListCategoriesAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty) return Failure<IReadOnlyCollection<ExpenseCategoryModel>>("TenantId must not be empty.");
        return Result<IReadOnlyCollection<ExpenseCategoryModel>>.Success(await categoryRepository.ListAsync(tenantId, cancellationToken));
    }

    public async Task<Result<ExpenseModel>> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.TenantId == Guid.Empty) return Failure<ExpenseModel>("TenantId must not be empty.");
            var category = await categoryRepository.GetAsync(request.TenantId, request.CategoryId, cancellationToken);
            if (category is null || !category.IsActive) return Result<ExpenseModel>.Failure(ApplicationError.Validation("An active expense category is required."));
            if (await expenseRepository.GetAsync(request.TenantId, request.Id, cancellationToken) is not null)
                return Result<ExpenseModel>.Failure(ApplicationError.Conflict("Expense already exists."));

            var expense = new Expense(request.Id, request.TenantId, request.CategoryId, request.ExpenseDate, request.Amount, request.Description);
            var model = Map(expense);
            await expenseRepository.AddAsync(model, cancellationToken);
            return Result<ExpenseModel>.Success(model);
        }
        catch (ArgumentException ex) { return Result<ExpenseModel>.Failure(ApplicationError.Validation(ex.Message)); }
    }

    public async Task<Result<ExpenseModel>> UpdateExpenseAsync(UpdateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.TenantId == Guid.Empty) return Failure<ExpenseModel>("TenantId must not be empty.");
            var category = await categoryRepository.GetAsync(request.TenantId, request.CategoryId, cancellationToken);
            if (category is null || !category.IsActive) return Result<ExpenseModel>.Failure(ApplicationError.Validation("An active expense category is required."));
            var existing = await expenseRepository.GetAsync(request.TenantId, request.ExpenseId, cancellationToken);
            if (existing is null) return Result<ExpenseModel>.Failure(ApplicationError.NotFound("Expense was not found."));

            var expense = new Expense(existing.Id, existing.TenantId, existing.CategoryId, existing.ExpenseDate, existing.Amount, existing.Description);
            expense.Update(request.CategoryId, request.ExpenseDate, request.Amount, request.Description);
            var model = Map(expense) with { BankTransactionId = existing.BankTransactionId };
            await expenseRepository.UpdateAsync(model, cancellationToken);
            return Result<ExpenseModel>.Success(model);
        }
        catch (ArgumentException ex) { return Result<ExpenseModel>.Failure(ApplicationError.Validation(ex.Message)); }
    }

    public async Task<Result<ExpenseModel>> GetExpenseAsync(Guid tenantId, Guid expenseId, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty || expenseId == Guid.Empty) return Failure<ExpenseModel>("TenantId and ExpenseId are required.");
        var expense = await expenseRepository.GetAsync(tenantId, expenseId, cancellationToken);
        return expense is null ? Result<ExpenseModel>.Failure(ApplicationError.NotFound("Expense was not found.")) : Result<ExpenseModel>.Success(expense);
    }

    public async Task<Result<IReadOnlyCollection<ExpenseModel>>> ListExpensesAsync(Guid tenantId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty) return Failure<IReadOnlyCollection<ExpenseModel>>("TenantId must not be empty.");
        if (from.HasValue && to.HasValue && from.Value.Date > to.Value.Date) return Failure<IReadOnlyCollection<ExpenseModel>>("From date must not be after To date.");
        return Result<IReadOnlyCollection<ExpenseModel>>.Success(await expenseRepository.ListAsync(tenantId, from?.Date, to?.Date, cancellationToken));
    }

    public async Task<Result<RecurringExpenseModel>> CreateRecurringExpenseAsync(CreateRecurringExpenseRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.TenantId == Guid.Empty) return Failure<RecurringExpenseModel>("TenantId must not be empty.");
            var category = await categoryRepository.GetAsync(request.TenantId, request.CategoryId, cancellationToken);
            if (category is null || !category.IsActive) return Result<RecurringExpenseModel>.Failure(ApplicationError.Validation("An active expense category is required."));
            if (await recurringExpenseRepository.GetAsync(request.TenantId, request.Id, cancellationToken) is not null)
                return Result<RecurringExpenseModel>.Failure(ApplicationError.Conflict("Recurring expense already exists."));
            var rule = new RecurringExpense(request.Id, request.TenantId, request.CategoryId, request.StartDate, request.Amount, request.Description, request.Frequency, request.Interval, request.EndDate);
            var model = new RecurringExpenseModel(rule.Id, rule.TenantId, rule.CategoryId, rule.StartDate, rule.Amount, rule.Description, rule.Frequency, rule.Interval, rule.EndDate, rule.IsActive);
            await recurringExpenseRepository.AddAsync(model, cancellationToken);
            return Result<RecurringExpenseModel>.Success(model);
        }
        catch (ArgumentException ex) { return Result<RecurringExpenseModel>.Failure(ApplicationError.Validation(ex.Message)); }
    }

    public async Task<Result<IReadOnlyCollection<DateTime>>> GetRecurringOccurrencesAsync(Guid tenantId, Guid recurringExpenseId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty || recurringExpenseId == Guid.Empty) return Failure<IReadOnlyCollection<DateTime>>("TenantId and RecurringExpenseId are required.");
        try
        {
            var model = await recurringExpenseRepository.GetAsync(tenantId, recurringExpenseId, cancellationToken);
            if (model is null) return Result<IReadOnlyCollection<DateTime>>.Failure(ApplicationError.NotFound("Recurring expense was not found."));
            var rule = new RecurringExpense(model.Id, model.TenantId, model.CategoryId, model.StartDate, model.Amount, model.Description, model.Frequency, model.Interval, model.EndDate);
            if (!model.IsActive) rule.Deactivate();
            return Result<IReadOnlyCollection<DateTime>>.Success(rule.GetOccurrences(from, to));
        }
        catch (ArgumentException ex) { return Result<IReadOnlyCollection<DateTime>>.Failure(ApplicationError.Validation(ex.Message)); }
    }

    public async Task<Result<ExpenseModel>> LinkBankTransactionAsync(LinkExpenseTransactionRequest request, CancellationToken cancellationToken = default)
    {
        if (request.TenantId == Guid.Empty || request.ExpenseId == Guid.Empty || request.BankTransactionId == Guid.Empty)
            return Failure<ExpenseModel>("TenantId, ExpenseId and BankTransactionId are required.");
        var existing = await expenseRepository.GetAsync(request.TenantId, request.ExpenseId, cancellationToken);
        if (existing is null) return Result<ExpenseModel>.Failure(ApplicationError.NotFound("Expense was not found."));
        await expenseRepository.LinkBankTransactionAsync(request.TenantId, request.ExpenseId, request.BankTransactionId, cancellationToken);
        return Result<ExpenseModel>.Success(existing with { BankTransactionId = request.BankTransactionId });
    }

    public async Task<Result<ExpenseReportModel>> GetReportAsync(Guid tenantId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty) return Failure<ExpenseReportModel>("TenantId must not be empty.");
        if (from.Date > to.Date) return Failure<ExpenseReportModel>("From date must not be after To date.");
        return Result<ExpenseReportModel>.Success(await reportingRepository.GetReportAsync(tenantId, from.Date, to.Date, cancellationToken));
    }

    private static Result<T> Failure<T>(string message) => Result<T>.Failure(ApplicationError.Validation(message));

    private static ExpenseModel Map(Expense expense) => new(expense.Id, expense.TenantId, expense.CategoryId, expense.ExpenseDate, expense.Amount, expense.Description, expense.BankTransactionId);
}
