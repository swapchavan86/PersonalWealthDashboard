namespace PersonalWealth.Application.Documents.Tabular;

public sealed record TabularTemplate(
    string TemplateId,
    int Version,
    IReadOnlyCollection<TabularField> Fields);
