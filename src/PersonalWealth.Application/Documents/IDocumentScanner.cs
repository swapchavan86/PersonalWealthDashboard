namespace PersonalWealth.Application.Documents;

public interface IDocumentScanner
{
    IReadOnlyList<DocumentDescriptor> Discover(CancellationToken cancellationToken = default);
}
