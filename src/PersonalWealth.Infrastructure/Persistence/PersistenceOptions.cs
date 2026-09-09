namespace PersonalWealth.Infrastructure.Persistence;

public sealed class PersistenceOptions
{
    public const string ConnectionStringName = "Default";

    public string ConnectionString { get; set; } = string.Empty;
}
