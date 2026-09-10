namespace PersonalWealth.Application.Documents.Business;

public interface ITransactionBusinessValidator
{
    BusinessValidationResult Validate(IReadOnlyCollection<NormalizedTransaction> transactions);
}
