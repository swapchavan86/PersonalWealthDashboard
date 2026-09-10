namespace PersonalWealth.Application.Documents;

public sealed record DocumentTemplate(
    string TemplateId,
    int Version,
    IReadOnlyCollection<string> SupportedExtensions);
