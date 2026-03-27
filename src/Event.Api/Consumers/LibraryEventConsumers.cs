using Contracts;
using EventService.Models;
using EventService.Repos;
using MassTransit;

namespace EventService.Consumers
{
    public class LibraryEventConsumer(
        IEventWriteRepository repo,
        ILogger<LibraryEventConsumer> logger) : IConsumer<LibraryEvent>
    {
        public async Task Consume(ConsumeContext<LibraryEvent> context)
        {
            try
            {
                var msg = context.Message;

                var doc = new EventDocument
                {
                    Id = msg.Id,
                    EntityType = msg.EntityType,
                    EntityId = msg.EntityId,
                    Action = msg.Action,
                    Timestamp = msg.Timestamp,
                    Metadata = msg.Metadata,
                    Version = msg.Version
                };
                await repo.InsertAsync(doc, context.CancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to store event {EventId} for entity {EntityId}", context.Message.Id, context.Message.EntityId);
                throw;
            }
        }
    }
}
