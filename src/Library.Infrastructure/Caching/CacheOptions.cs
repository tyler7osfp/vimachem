namespace Library.Infrastructure.Caching;

public class CacheOptions
{
    public const string SectionName = "Cache";

    public TimeSpan Ttl { get; set; } = TimeSpan.FromMinutes(5);
}
