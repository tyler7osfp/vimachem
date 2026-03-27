namespace Microsoft.Extensions.Configuration;

public static class ConfigurationConnectionStringExtensions
{
    public static string GetRequiredMongoConnectionString(this IConfiguration configuration)
    {
        return configuration.GetConnectionString("eventdb")
            ?? configuration["MongoDB:ConnectionString"]
            ?? throw new InvalidOperationException(
                "No MongoDB connection string. Set ConnectionStrings:eventdb (Aspire) or MongoDB:ConnectionString (Docker Compose).");
    }

    public static string GetRequiredPostgresConnectionString(this IConfiguration configuration)
    {
        var s = configuration.GetConnectionString("librarydb")
            ?? configuration.GetConnectionString("Postgres");
        if (string.IsNullOrWhiteSpace(s))
            throw new InvalidOperationException(
                "No PostgreSQL connection string. Set ConnectionStrings:librarydb (Aspire) or ConnectionStrings:Postgres (Docker Compose).");
        return s;
    }
}
