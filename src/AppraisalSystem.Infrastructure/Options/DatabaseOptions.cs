namespace AppraisalSystem.Infrastructure.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Provider { get; init; } = "Sqlite";
    public string ConnectionString { get; init; } = "Data Source=appraisal.db";
}