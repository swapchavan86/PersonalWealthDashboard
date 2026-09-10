using PersonalWealth.Application.Documents;

namespace PersonalWealth.Infrastructure.Documents;

public sealed class DocumentTemplateSelector : IDocumentTemplateSelector
{
    public DocumentTemplate? Select(string extension, IReadOnlyCollection<DocumentTemplate> templates)
    {
        var normalizedExtension = extension.StartsWith('.') ? extension.ToLowerInvariant() : "." + extension.ToLowerInvariant();
        return templates
            .Where(x => x.SupportedExtensions.Any(e => string.Equals(e, normalizedExtension, StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(x => x.Version)
            .FirstOrDefault();
    }
}
