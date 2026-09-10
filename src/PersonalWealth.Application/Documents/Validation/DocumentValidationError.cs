namespace PersonalWealth.Application.Documents.Validation;

public sealed record DocumentValidationError(
    string Code,
    string Message,
    int? RowNumber = null,
    string? Field = null);
