namespace PersonalWealth.Application.Documents.Business;

public sealed record BusinessValidationError(string Code, string Message);

public sealed record BusinessValidationResult(IReadOnlyCollection<BusinessValidationError> Errors)
{
    public bool IsValid => Errors.Count == 0;
}
