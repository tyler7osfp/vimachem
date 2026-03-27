namespace EventService;

public class EventCleanupOptions
{
    public const string SectionName = "EventCleanup";

    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(24);
    public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(365);
}
