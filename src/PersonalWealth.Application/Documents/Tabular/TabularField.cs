namespace PersonalWealth.Application.Documents.Tabular;

public enum TabularFieldType
{
    Text,
    Date,
    Decimal
}

public enum AmountSemantic
{
    None,
    Debit,
    Credit,
    SignedAmount
}

public sealed record TabularField(
    string TargetField,
    string SourceColumn,
    bool Required,
    TabularFieldType Type,
    string? Format,
    string? Normalization,
    AmountSemantic AmountSemantic = AmountSemantic.None);
