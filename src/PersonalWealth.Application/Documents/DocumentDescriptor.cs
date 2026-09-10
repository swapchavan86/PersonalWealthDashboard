namespace PersonalWealth.Application.Documents;

public sealed record DocumentDescriptor(
    string FullPath,
    string FileName,
    string Extension,
    long Length,
    DateTimeOffset LastModifiedUtc);
