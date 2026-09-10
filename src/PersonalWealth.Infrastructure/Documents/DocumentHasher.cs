using System.Security.Cryptography;
using PersonalWealth.Application.Documents;

namespace PersonalWealth.Infrastructure.Documents;

public sealed class DocumentHasher : IDocumentHasher
{
    public async Task<string> ComputeSha256Async(
        string fullPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullPath);

        await using var stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            81920,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash);
    }
}
