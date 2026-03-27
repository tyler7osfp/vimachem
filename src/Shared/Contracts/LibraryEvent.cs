namespace Contracts;
public record LibraryEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid EntityId { get; init; }
    public EntityType EntityType { get; init; }
    public string Action { get; init; } = string.Empty;
    public DateTime Timestamp  { get; init; } = DateTime.UtcNow;
    public Dictionary<string, string> Metadata { get; init; } = new();
    public int Version { get; init; } = 1;
}