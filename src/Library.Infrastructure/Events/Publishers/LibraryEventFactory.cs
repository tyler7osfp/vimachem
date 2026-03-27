using Contracts;

namespace Library.Infrastructure.Events.Publishers;

public static class LibraryEventFactory
{
    public static LibraryEvent Create(Guid entityId, EntityType entityType, string action,
        Dictionary<string, string>? metadata = null)
        => new()
        {
            EntityId = entityId,
            EntityType = entityType,
            Action = action,
            Metadata = metadata ?? []
        };
}
