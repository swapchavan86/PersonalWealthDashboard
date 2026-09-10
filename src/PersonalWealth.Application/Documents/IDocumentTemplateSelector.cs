namespace PersonalWealth.Application.Documents;

public interface IDocumentTemplateSelector
{
    DocumentTemplate? Select(string extension, IReadOnlyCollection<DocumentTemplate> templates);
}
