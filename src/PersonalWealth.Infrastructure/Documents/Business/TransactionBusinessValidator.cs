using PersonalWealth.Application.Documents.Business;

namespace PersonalWealth.Infrastructure.Documents.Business;

public sealed class TransactionBusinessValidator : ITransactionBusinessValidator
{
    public BusinessValidationResult Validate(IReadOnlyCollection<NormalizedTransaction> transactions)
    {
        var errors = new List<BusinessValidationError>();

        foreach (var transaction in transactions)
        {
            if (transaction.TransactionDate == default)
                errors.Add(new("INVALID_TRANSACTION_DATE", "Transaction date is required."));
            if (transaction.Amount == 0)
                errors.Add(new("ZERO_TRANSACTION_AMOUNT", "Transaction amount cannot be zero."));
            if (string.IsNullOrWhiteSpace(transaction.Description))
                errors.Add(new("MISSING_TRANSACTION_DESCRIPTION", "Transaction description is required."));
        }

        return new BusinessValidationResult(errors);
    }
}
