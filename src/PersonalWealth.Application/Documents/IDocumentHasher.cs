namespace PersonalWealth.Application.Documents;

public interface IDocumentHasher
{
    Task<string> ComputeSha256Async(string fullPath, CancellationToken cancellationToken = default);
}
