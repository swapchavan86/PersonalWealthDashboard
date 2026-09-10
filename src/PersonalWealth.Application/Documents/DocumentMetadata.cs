namespace PersonalWealth.Application.Documents;

public sealed record DocumentMetadata(
    Guid DocumentId,
    string FileName,
    string FullPath,
    string ContentHash,
    long Length,
    DateTimeOffset LastModifiedUtc,
    string Status);
